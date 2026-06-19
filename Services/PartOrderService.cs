using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Repositories;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;

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
            // Validate delivery method and address
            if (string.IsNullOrEmpty(order.DeliveryMethod))
            {
                order.DeliveryMethod = "Pickup";
            }

            if (order.DeliveryMethod == "HomeDelivery" && string.IsNullOrWhiteSpace(order.ShippingAddress))
            {
                throw new InvalidOperationException("Địa chỉ giao hàng là bắt buộc khi chọn phương thức Giao hàng tận nơi.");
            }

            // Set shipping fee dynamically based on delivery method
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
                foreach (var detail in order.PartOrderDetails)
                {
                    var part = context.Parts.SingleOrDefault(p => p.PartId == detail.PartId);
                    if (part == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy phụ tùng với ID: {detail.PartId}");
                    }
                    if (part.Quantity < detail.Quantity)
                    {
                        throw new InvalidOperationException($"Số lượng tồn kho cho phụ tùng '{part.PartName}' không đủ (Chỉ còn {part.Quantity} sản phẩm).");
                    }
                    if (part.Status != "Available" && part.Status != "Available")
                    {
                        throw new InvalidOperationException($"Phụ tùng '{part.PartName}' hiện không sẵn sàng để bán.");
                    }

                    // Decrement stock
                    part.Quantity -= detail.Quantity;
                    if (part.Quantity == 0)
                    {
                        part.Status = "Out of Stock";
                    }

                    detail.UnitPrice = part.Price;
                    detail.SubTotal = part.Price * detail.Quantity;
                    total += detail.SubTotal;

                    context.Entry(part).State = EntityState.Modified;
                }

                order.TotalAmount = total + order.ShippingFee;
                order.CreatedAt = DateTime.Now;
                order.Status = "Pending";

                context.PartOrders.Add(order);
                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
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

                // Check if transitioning to Cancelled
                if (order.Status == "Cancelled" && dbOrder.Status != "Cancelled")
                {
                    // Restore stock
                    foreach (var detail in dbOrder.PartOrderDetails)
                    {
                        var part = context.Parts.SingleOrDefault(p => p.PartId == detail.PartId);
                        if (part != null)
                        {
                            part.Quantity += detail.Quantity;
                            if (part.Status == "Out of Stock" && part.Quantity > 0)
                            {
                                part.Status = "Available";
                            }
                            context.Entry(part).State = EntityState.Modified;
                        }
                    }
                }

                // Update properties
                dbOrder.Status = order.Status;
                dbOrder.UpdatedAt = DateTime.Now;
                if (dbOrder.Status != null) dbOrder.Status = order.Status;
                if (order.CustomerName != null) dbOrder.CustomerName = order.CustomerName;
                if (order.CustomerPhone != null) dbOrder.CustomerPhone = order.CustomerPhone;
                if (order.CustomerEmail != null) dbOrder.CustomerEmail = order.CustomerEmail;
                if (order.ShippingAddress != null) dbOrder.ShippingAddress = order.ShippingAddress;
                if (order.DeliveryMethod != null) dbOrder.DeliveryMethod = order.DeliveryMethod;
                if (order.ShippingFee != 0) dbOrder.ShippingFee = order.ShippingFee;

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
