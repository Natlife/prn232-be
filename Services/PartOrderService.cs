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
                throw new InvalidOperationException("Dia chi giao hang la bat buoc khi chon Giao hang tan noi.");
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
                foreach (var detail in order.PartOrderDetails)
                {
                    var part = context.Parts.SingleOrDefault(p => p.PartId == detail.PartId);
                    if (part == null)
                    {
                        throw new InvalidOperationException($"Khong tim thay phu tung voi ID: {detail.PartId}");
                    }

                    if (part.Quantity < detail.Quantity)
                    {
                        throw new InvalidOperationException($"So luong ton kho cho phu tung '{part.PartName}' khong du.");
                    }

                    if (part.Status != "Available")
                    {
                        throw new InvalidOperationException($"Phu tung '{part.PartName}' hien khong san sang de ban.");
                    }

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
                    throw new InvalidOperationException("Khong tim thay don hang can cap nhat.");
                }

                if (order.Status == "Cancelled" && dbOrder.Status != "Cancelled")
                {
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
