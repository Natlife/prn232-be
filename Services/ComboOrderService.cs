using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using DataAccessObjects;
using Repositories;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class ComboOrderService : IComboOrderService
    {
        private readonly IComboOrderRepository _repository;

        public ComboOrderService(IComboOrderRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ComboOrder> GetAllOrders() => _repository.GetAllOrders();

        public ComboOrder? GetOrderById(int orderId) => _repository.GetOrderById(orderId);

        public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId) => _repository.GetOrdersByCustomerId(customerId);

        public void AddOrder(ComboOrder order)
        {
            order.Status = "Pending";
            order.CreatedAt = DateTime.Now;
            order.IsCaptchaUsed = false;
            order.IsFinalCaptchaUsed = false;

            if (string.Equals(order.PurchaseType, "Deposit", StringComparison.OrdinalIgnoreCase))
            {
                order.DepositAmount = Math.Round(order.TotalAmount * 0.05m, 0);
            }
            else
            {
                order.PurchaseType = "Buyout";
                order.DepositAmount = null;
            }

            _repository.AddOrder(order);
        }

        public void UpdateOrder(ComboOrder order)
        {
            order.UpdatedAt = DateTime.Now;
            _repository.UpdateOrder(order);
        }

        public void GenerateCaptcha(int orderId, string? customCode)
        {
            var order = _repository.GetOrderById(orderId);
            if (order == null) throw new ArgumentException("Không tìm thấy đơn hàng combo.");

            string code = string.IsNullOrWhiteSpace(customCode) 
                ? GenerateRandomCode() 
                : customCode.Trim().ToUpper();

            if (string.Equals(order.PurchaseType, "Deposit", StringComparison.OrdinalIgnoreCase))
            {
                order.CaptchaCode = code;
                order.CaptchaGeneratedAt = DateTime.Now;
                order.DepositExpiresAt = DateTime.Now.AddDays(3);
                order.IsCaptchaUsed = false;
            }
            else
            {
                order.FinalCaptchaCode = code;
                order.FinalCaptchaGeneratedAt = DateTime.Now;
                order.IsFinalCaptchaUsed = false;
            }

            order.UpdatedAt = DateTime.Now;
            _repository.UpdateOrder(order);
        }

        public bool VerifyCaptcha(int orderId, string captchaCode)
        {
            var order = _repository.GetOrderById(orderId);
            if (order == null) throw new ArgumentException("Không tìm thấy đơn hàng combo.");

            string inputCode = captchaCode.Trim().ToUpper();

            using var context = new CarShowroomContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var trackedOrder = context.ComboOrders.Include(o => o.Items).SingleOrDefault(o => o.ComboOrderId == orderId);
                if (trackedOrder == null) throw new ArgumentException("Không tìm thấy đơn hàng combo.");

                if (string.Equals(trackedOrder.PurchaseType, "Deposit", StringComparison.OrdinalIgnoreCase))
                {
                    if (trackedOrder.IsCaptchaUsed)
                        throw new InvalidOperationException("Mã xác nhận này đã được sử dụng trước đó.");
                    if (!string.Equals(trackedOrder.CaptchaCode, inputCode, StringComparison.OrdinalIgnoreCase))
                        return false;

                    trackedOrder.IsCaptchaUsed = true;
                    trackedOrder.CaptchaUsedAt = DateTime.Now;
                    trackedOrder.Status = "Paid";
                }
                else
                {
                    if (trackedOrder.IsFinalCaptchaUsed)
                        throw new InvalidOperationException("Mã xác nhận này đã được sử dụng trước đó.");
                    if (!string.Equals(trackedOrder.FinalCaptchaCode, inputCode, StringComparison.OrdinalIgnoreCase))
                        return false;

                    trackedOrder.IsFinalCaptchaUsed = true;
                    trackedOrder.FinalCaptchaUsedAt = DateTime.Now;
                    trackedOrder.Status = "Paid";
                }

                trackedOrder.UpdatedAt = DateTime.Now;

                foreach (var item in trackedOrder.Items)
                {
                    if (string.Equals(item.ItemType, "Car", StringComparison.OrdinalIgnoreCase))
                    {
                        var car = context.Cars.SingleOrDefault(c => c.CarId == item.ReferenceId);
                        if (car != null)
                        {
                            if (string.Equals(trackedOrder.PurchaseType, "Deposit", StringComparison.OrdinalIgnoreCase))
                            {
                                car.Status = "Reserved";
                            }
                            else
                            {
                                car.Status = "Sold";
                            }
                        }
                    }
                    else if (string.Equals(item.ItemType, "Part", StringComparison.OrdinalIgnoreCase))
                    {
                        var part = context.Parts.SingleOrDefault(p => p.PartId == item.ReferenceId);
                        if (part != null)
                        {
                            if (part.Quantity < item.Quantity)
                            {
                                throw new InvalidOperationException($"Phụ tùng '{part.PartName}' không đủ tồn kho (Còn lại: {part.Quantity}, Yêu cầu: {item.Quantity}).");
                            }
                            part.Quantity -= item.Quantity;

                            var trans = new InventoryTransaction
                            {
                                PartId = part.PartId,
                                TransactionType = "Export",
                                Quantity = -item.Quantity,
                                ReferenceType = "ComboOrder",
                                ReferenceId = orderId,
                                StaffId = 1,
                                Notes = $"Xuất kho cho đơn hàng combo #{orderId}",
                                TransactionDate = DateTime.Now
                            };
                            context.InventoryTransactions.Add(trans);
                        }
                    }
                }

                context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void CancelOrder(int orderId)
        {
            var order = _repository.GetOrderById(orderId);
            if (order == null) throw new ArgumentException("Không tìm thấy đơn hàng combo.");

            if (order.Status == "Paid")
                throw new InvalidOperationException("Không thể hủy đơn hàng đã thanh toán.");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.Now;
            _repository.UpdateOrder(order);
        }

        private string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
