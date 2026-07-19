using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Services
{
    public class PartOrderService : IPartOrderService
    {
        private readonly IPartOrderRepository _orderRepository;

        public PartOrderService(IPartOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<PartOrder> GetAllOrders() => _orderRepository.GetAllOrders();

        public PartOrder? GetOrderById(int orderId) => _orderRepository.GetOrderById(orderId);

        public IEnumerable<PartOrder> GetOrdersByCustomerId(int customerId) => _orderRepository.GetOrdersByCustomerId(customerId);

        public void AddOrder(PartOrder order)
        {
            if (string.IsNullOrEmpty(order.DeliveryMethod))
            {
                order.DeliveryMethod = "Pickup";
            }

            if (order.DeliveryMethod == "HomeDelivery" && string.IsNullOrWhiteSpace(order.ShippingAddress))
            {
                throw new InvalidOperationException("Địa chỉ giao hàng là bắt buộc khi chọn Giao hàng tận nơi.");
            }

            order.ShippingFee = order.DeliveryMethod switch
            {
                "HomeDelivery" => 30000m,
                "GarageInstallation" => 50000m,
                _ => 0m
            };

            using var context = new CarShowroomContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                decimal total = 0;
                var pendingTransactions = new List<InventoryTransaction>();
                var splitDetails = new List<PartOrderDetail>();
                
                // Copy details list to iterate to avoid collection modification exceptions
                var originalDetails = order.PartOrderDetails.ToList();

                foreach (var detail in originalDetails)
                {
                    var part = context.Parts.SingleOrDefault(p => p.PartId == detail.PartId);
                    if (part == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy phụ tùng với ID: {detail.PartId}");
                    }

                    if (part.Status == "Inactive")
                    {
                        throw new InvalidOperationException($"Phụ tùng '{part.PartName}' hiện không sẵn sàng để bán.");
                    }

                    var remainingQuantity = detail.Quantity;
                    
                    // Implement FIFO/FEFO: Order candidate parts of the same generic name by ExpiredAt ascending (earliest first, nulls/non-perishables last)
                    var candidates = context.Parts
                        .Where(p => p.PartName == part.PartName && p.Status == "Available" && p.Quantity > 0)
                        .OrderBy(p => p.ExpiredAt ?? DateTime.MaxValue)
                        .ToList();

                    if (!candidates.Any())
                    {
                        candidates.Add(part);
                    }

                    bool firstAllocated = false;
                    foreach (var candidate in candidates)
                    {
                        if (remainingQuantity <= 0) break;

                        int allocQty = Math.Min(candidate.Quantity, remainingQuantity);
                        if (allocQty <= 0) continue;

                        candidate.Quantity -= allocQty;
                        remainingQuantity -= allocQty;

                        // Apply MinStockLevel warning & status update
                        if (candidate.Quantity == 0 || candidate.Quantity < candidate.MinStockLevel)
                        {
                            candidate.Status = "OutOfStock";
                        }

                        context.Entry(candidate).State = EntityState.Modified;

                        if (!firstAllocated)
                        {
                            detail.PartId = candidate.PartId;
                            detail.Quantity = allocQty;
                            detail.UnitPrice = candidate.Price;
                            detail.SubTotal = candidate.Price * allocQty;
                            total += detail.SubTotal;
                            firstAllocated = true;
                        }
                        else
                        {
                            var splitDetail = new PartOrderDetail
                            {
                                PartId = candidate.PartId,
                                Quantity = allocQty,
                                UnitPrice = candidate.Price,
                                SubTotal = candidate.Price * allocQty
                            };
                            splitDetails.Add(splitDetail);
                            total += splitDetail.SubTotal;
                        }

                        // Prepare InventoryTransaction (Audit Trail)
                        var invTx = new InventoryTransaction
                        {
                            PartId = candidate.PartId,
                            TransactionType = "Export",
                            Quantity = -allocQty, // Negative for export
                            ReferenceType = "PartOrder",
                            StaffId = order.CustomerId,
                            Notes = $"Xuất kho bán phụ tùng cho đơn hàng",
                            TransactionDate = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            CreatedUser = order.CustomerId
                        };
                        pendingTransactions.Add(invTx);
                    }

                    if (remainingQuantity > 0)
                    {
                        throw new InvalidOperationException($"Số lượng tồn kho cho phụ tùng '{part.PartName}' không đủ.");
                    }
                }

                // Add split details if any
                foreach (var sd in splitDetails)
                {
                    order.PartOrderDetails.Add(sd);
                }

                order.TotalAmount = total + order.ShippingFee;
                order.CreatedAt = DateTime.Now;
                order.Status = "Pending";

                context.PartOrders.Add(order);
                context.SaveChanges(); // Generates order.OrderId

                // Now associate transactions with order.OrderId and save
                foreach (var tx in pendingTransactions)
                {
                    tx.ReferenceId = order.OrderId;
                    tx.Notes += $" #{order.OrderId}";
                    context.InventoryTransactions.Add(tx);
                }

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ExceptionMessageHelper.GetDetailedMessage(ex), ex);
            }
        }

        public void UpdateOrder(PartOrder order)
        {
            using var context = new CarShowroomContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var dbOrder = context.PartOrders
                    .Include(o => o.PartOrderDetails)
                    .SingleOrDefault(o => o.OrderId == order.OrderId);

                if (dbOrder == null)
                {
                    throw new InvalidOperationException("Không tìm thấy đơn hàng cần cập nhật.");
                }

                if (order.Status == "Cancelled" && dbOrder.Status != "Cancelled")
                {
                    foreach (var detail in dbOrder.PartOrderDetails)
                    {
                        var part = context.Parts.SingleOrDefault(p => p.PartId == detail.PartId);
                        if (part != null)
                        {
                            part.Quantity += detail.Quantity;
                            if ((part.Status == "OutOfStock" || part.Status == "Out of Stock") && part.Quantity > 0)
                            {
                                part.Status = "Available";
                            }

                            context.Entry(part).State = EntityState.Modified;

                            // Log Return InventoryTransaction (Audit Trail)
                            var invTx = new InventoryTransaction
                            {
                                PartId = part.PartId,
                                TransactionType = "Return",
                                Quantity = detail.Quantity, // Positive for return
                                ReferenceType = "PartOrder",
                                ReferenceId = dbOrder.OrderId,
                                StaffId = dbOrder.CustomerId,
                                Notes = $"Khách hàng trả hàng / Hủy đơn hàng #{dbOrder.OrderId}",
                                TransactionDate = DateTime.Now,
                                CreatedAt = DateTime.Now,
                                CreatedUser = dbOrder.CustomerId
                            };
                            context.InventoryTransactions.Add(invTx);
                        }
                    }
                }

                dbOrder.Status = order.Status;
                dbOrder.UpdatedAt = DateTime.Now;
                if (order.CustomerName != null) dbOrder.CustomerName = order.CustomerName;
                if (order.CustomerPhone != null) dbOrder.CustomerPhone = order.CustomerPhone;
                if (order.CustomerEmail != null) dbOrder.CustomerEmail = order.CustomerEmail;
                if (order.ShippingAddress != null) dbOrder.ShippingAddress = order.ShippingAddress;
                if (order.DeliveryMethod != null) dbOrder.DeliveryMethod = order.DeliveryMethod;
                if (order.ShippingFee != 0) dbOrder.ShippingFee = order.ShippingFee;

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ExceptionMessageHelper.GetDetailedMessage(ex), ex);
            }
        }
    }
}
