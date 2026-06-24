using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

// ─── FLOW ─────────────────────────────────────────────────────────────────────
//
//  Singleton pattern — consistent with existing DAOs in this codebase.
//  Each public method opens a scoped CarShowroomContext via `using`
//  to ensure connection is returned to pool immediately after use.
//
//  GetById / GetByCustomer → Include Items for full graph
//  AddOrder               → sets server-side fields (CreatedAt, Status)
//  UpdateStatus           → targeted update — only touches Status + UpdatedAt
//
// ─────────────────────────────────────────────────────────────────────────────

public class ComboOrderDAO
{
    private static ComboOrderDAO? _instance;
    private static readonly object _lock = new();

    private ComboOrderDAO() { }

    public static ComboOrderDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new ComboOrderDAO();
            }
        }
    }

    public IEnumerable<ComboOrder> GetAllOrders()
    {
        using var context = new CarShowroomContext();
        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId)
    {
        using var context = new CarShowroomContext();
        return context.ComboOrders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    public ComboOrder? GetOrderById(int comboOrderId)
    {
        using var context = new CarShowroomContext();
        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .SingleOrDefault(o => o.ComboOrderId == comboOrderId);
    }

    public void AddOrder(ComboOrder order)
    {
        using var context = new CarShowroomContext();
        order.CreatedAt = DateTime.Now;
        order.Status = "Pending";
        order.IsCaptchaUsed = false;
        context.ComboOrders.Add(order);
        context.SaveChanges();
    }

    public void UpdateStatus(int comboOrderId, string newStatus)
    {
        using var context = new CarShowroomContext();
        var order = context.ComboOrders.SingleOrDefault(o => o.ComboOrderId == comboOrderId);
        if (order is null)
            throw new InvalidOperationException($"ComboOrder {comboOrderId} not found.");

        order.Status = newStatus;
        order.UpdatedAt = DateTime.Now;
        context.Entry(order).State = EntityState.Modified;
        context.SaveChanges();
    }

    public ComboOrder GenerateCaptcha(int comboOrderId, string? code)
    {
        using var context = new CarShowroomContext();
        var order = context.ComboOrders
            .Include(o => o.Items)
            .SingleOrDefault(o => o.ComboOrderId == comboOrderId);

        if (order is null)
            throw new InvalidOperationException($"ComboOrder {comboOrderId} not found.");

        if (string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Đơn hàng đã bị hủy, không thể tạo captcha.");

        if (order.IsCaptchaUsed)
            throw new InvalidOperationException("Đơn hàng này đã xác thực captcha trước đó.");

        var finalCode = string.IsNullOrWhiteSpace(code)
            ? GenerateRandomCode(context)
            : code.Trim().ToUpperInvariant();

        var duplicate = context.ComboOrders.Any(o =>
            o.ComboOrderId != comboOrderId &&
            o.CaptchaCode != null &&
            o.CaptchaCode == finalCode);
        if (duplicate)
            throw new InvalidOperationException("Mã captcha này đã tồn tại trong hệ thống.");

        order.CaptchaCode = finalCode;
        order.CaptchaGeneratedAt = DateTime.Now;
        order.CaptchaUsedAt = null;
        order.IsCaptchaUsed = false;
        order.UpdatedAt = DateTime.Now;

        context.SaveChanges();
        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Single(o => o.ComboOrderId == comboOrderId);
    }

    public ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode)
    {
        using var context = new CarShowroomContext();
        using var transaction = context.Database.BeginTransaction();

        var order = context.ComboOrders
            .Include(o => o.Items)
            .SingleOrDefault(o => o.ComboOrderId == comboOrderId);

        if (order is null)
            throw new InvalidOperationException("Không tìm thấy đơn hàng combo.");

        if (order.CustomerId != customerId)
            throw new InvalidOperationException("Bạn không có quyền xác thực đơn hàng này.");

        if (string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Đơn hàng đã bị hủy.");

        if (order.IsCaptchaUsed)
            throw new InvalidOperationException("Đơn hàng này đã được xác thực captcha.");

        if (string.IsNullOrWhiteSpace(order.CaptchaCode))
            throw new InvalidOperationException("Admin chưa tạo captcha cho đơn hàng này.");

        if (!string.Equals(order.CaptchaCode, captchaCode?.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Mã captcha không hợp lệ.");

        var now = DateTime.Now;
        var normalizedType = (order.PurchaseType ?? "Buyout").Trim();

        foreach (var item in order.Items)
        {
            if (string.Equals(item.ItemType, "Car", StringComparison.OrdinalIgnoreCase))
            {
                var car = context.Cars.SingleOrDefault(c => c.CarId == item.ReferenceId)
                    ?? throw new InvalidOperationException($"Không tìm thấy xe với ID {item.ReferenceId}.");

                if (string.Equals(normalizedType, "Deposit", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(car.Status, "Available", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException($"Xe '{car.CarName}' hiện không sẵn sàng để đặt cọc.");

                    car.Status = "Reserved";
                }
                else
                {
                    if (string.Equals(car.Status, "Sold", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException($"Xe '{car.CarName}' đã được bán.");

                    car.Status = "Sold";
                }
            }
            else if (string.Equals(item.ItemType, "Part", StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(normalizedType, "Buyout", StringComparison.OrdinalIgnoreCase))
            {
                var part = context.Parts.SingleOrDefault(p => p.PartId == item.ReferenceId)
                    ?? throw new InvalidOperationException($"Không tìm thấy phụ tùng với ID {item.ReferenceId}.");

                if (part.Quantity < item.Quantity)
                    throw new InvalidOperationException($"Phụ tùng '{part.PartName}' không đủ tồn kho để hoàn tất đơn.");

                part.Quantity -= item.Quantity;
            }
        }

        order.IsCaptchaUsed = true;
        order.CaptchaUsedAt = now;
        order.UpdatedAt = now;
        order.Status = string.Equals(normalizedType, "Deposit", StringComparison.OrdinalIgnoreCase)
            ? "Deposited"
            : "Completed";

        context.SaveChanges();
        transaction.Commit();

        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Single(o => o.ComboOrderId == comboOrderId);
    }

    private static string GenerateRandomCode(CarShowroomContext context)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code;
        do
        {
            code = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        } while (context.ComboOrders.Any(o => o.CaptchaCode == code));

        return code;
    }
}
