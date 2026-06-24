using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class ComboOrderDAO
{
    private const int DepositHoldDays = 7;

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
        order.IsFinalCaptchaUsed = false;
        order.DepositExpiresAt = null;
        context.ComboOrders.Add(order);
        context.SaveChanges();
    }

    public void UpdateStatus(int comboOrderId, string newStatus)
    {
        using var context = new CarShowroomContext();
        var order = context.ComboOrders.SingleOrDefault(o => o.ComboOrderId == comboOrderId);
        if (order is null)
        {
            throw new InvalidOperationException($"ComboOrder {comboOrderId} not found.");
        }

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
        {
            throw new InvalidOperationException($"ComboOrder {comboOrderId} not found.");
        }

        EnsureNotClosed(order);

        var stage = ResolveCaptchaStage(order);
        var finalCode = string.IsNullOrWhiteSpace(code)
            ? GenerateRandomCode(context)
            : code.Trim().ToUpperInvariant();

        if (stage == "buyout-final" && string.Equals(finalCode, order.CaptchaCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Captcha mua dut phai khac captcha dat coc lan dau.");
        }

        if (ExistsAnyCaptcha(context, comboOrderId, finalCode))
        {
            throw new InvalidOperationException("Ma captcha nay da ton tai trong he thong.");
        }

        var now = DateTime.Now;
        if (stage == "deposit")
        {
            order.CaptchaCode = finalCode;
            order.CaptchaGeneratedAt = now;
            order.CaptchaUsedAt = null;
            order.IsCaptchaUsed = false;
        }
        else
        {
            order.FinalCaptchaCode = finalCode;
            order.FinalCaptchaGeneratedAt = now;
            order.FinalCaptchaUsedAt = null;
            order.IsFinalCaptchaUsed = false;
        }

        order.UpdatedAt = now;
        context.SaveChanges();
        return LoadOrder(context, comboOrderId);
    }

    public ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode)
    {
        using var context = new CarShowroomContext();
        using var transaction = context.Database.BeginTransaction();

        var order = context.ComboOrders
            .Include(o => o.Items)
            .SingleOrDefault(o => o.ComboOrderId == comboOrderId);

        if (order is null)
        {
            throw new InvalidOperationException("Khong tim thay don hang combo.");
        }

        if (order.CustomerId != customerId)
        {
            throw new InvalidOperationException("Ban khong co quyen xu ly don hang nay.");
        }

        EnsureNotClosed(order);

        var now = DateTime.Now;
        if (IsDepositExpired(order, now))
        {
            ReleaseReservedCars(context, order);
            order.Status = "DepositExpired";
            order.UpdatedAt = now;
            context.SaveChanges();
            transaction.Commit();
            throw new InvalidOperationException("Don dat coc combo da het han giu cho sau 7 ngay.");
        }

        var normalizedType = NormalizePurchaseType(order.PurchaseType);
        if (normalizedType == "Buyout")
        {
            VerifyPrimaryCaptcha(order, captchaCode);
            CompleteBuyout(context, order, now);
        }
        else if (order.Status == "Pending")
        {
            VerifyPrimaryCaptcha(order, captchaCode);
            CompleteDeposit(context, order, now);
        }
        else if (order.Status == "Deposited")
        {
            VerifyFinalCaptcha(order, captchaCode);
            CompleteBuyout(context, order, now);
        }
        else
        {
            throw new InvalidOperationException("Don combo hien khong o trang thai co the xac thuc.");
        }

        context.SaveChanges();
        transaction.Commit();
        return LoadOrder(context, comboOrderId);
    }

    private static ComboOrder LoadOrder(CarShowroomContext context, int comboOrderId)
    {
        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Single(o => o.ComboOrderId == comboOrderId);
    }

    private static void EnsureNotClosed(ComboOrder order)
    {
        if (string.Equals(order.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Don hang da bi huy.");
        }

        if (string.Equals(order.Status, "Completed", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Don hang combo nay da hoan tat.");
        }

        if (string.Equals(order.Status, "DepositExpired", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Don dat coc combo nay da het han.");
        }
    }

    private static string ResolveCaptchaStage(ComboOrder order)
    {
        var normalizedType = NormalizePurchaseType(order.PurchaseType);
        if (normalizedType == "Buyout")
        {
            if (order.IsCaptchaUsed)
            {
                throw new InvalidOperationException("Don mua dut nay da xac thuc captcha truoc do.");
            }

            return "buyout";
        }

        if (string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return "deposit";
        }

        if (string.Equals(order.Status, "Deposited", StringComparison.OrdinalIgnoreCase))
        {
            if (order.IsFinalCaptchaUsed)
            {
                throw new InvalidOperationException("Don combo nay da xac thuc captcha mua dut.");
            }

            return "buyout-final";
        }

        throw new InvalidOperationException("Khong the tao captcha cho trang thai hien tai cua don combo.");
    }

    private static bool ExistsAnyCaptcha(CarShowroomContext context, int comboOrderId, string code)
    {
        return context.ComboOrders.Any(o =>
            o.ComboOrderId != comboOrderId &&
            ((o.CaptchaCode != null && o.CaptchaCode == code) ||
             (o.FinalCaptchaCode != null && o.FinalCaptchaCode == code)));
    }

    private static string NormalizePurchaseType(string? purchaseType)
    {
        return string.Equals(purchaseType, "Deposit", StringComparison.OrdinalIgnoreCase)
            ? "Deposit"
            : "Buyout";
    }

    private static bool IsDepositExpired(ComboOrder order, DateTime now)
    {
        return string.Equals(order.Status, "Deposited", StringComparison.OrdinalIgnoreCase)
            && order.DepositExpiresAt.HasValue
            && order.DepositExpiresAt.Value < now;
    }

    private static void VerifyPrimaryCaptcha(ComboOrder order, string captchaCode)
    {
        if (order.IsCaptchaUsed)
        {
            throw new InvalidOperationException("Captcha lan dau cua don combo nay da duoc su dung.");
        }

        if (string.IsNullOrWhiteSpace(order.CaptchaCode))
        {
            throw new InvalidOperationException("Admin chua tao captcha lan dau cho don combo nay.");
        }

        if (!string.Equals(order.CaptchaCode, captchaCode?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Ma captcha khong hop le.");
        }
    }

    private static void VerifyFinalCaptcha(ComboOrder order, string captchaCode)
    {
        if (order.IsFinalCaptchaUsed)
        {
            throw new InvalidOperationException("Captcha mua dut lan hai da duoc su dung.");
        }

        if (string.IsNullOrWhiteSpace(order.FinalCaptchaCode))
        {
            throw new InvalidOperationException("Admin chua tao captcha lan hai de mua dut don combo nay.");
        }

        if (string.Equals(order.FinalCaptchaCode, order.CaptchaCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Captcha mua dut phai khac captcha dat coc.");
        }

        if (!string.Equals(order.FinalCaptchaCode, captchaCode?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Ma captcha mua dut khong hop le.");
        }
    }

    private static void CompleteDeposit(CarShowroomContext context, ComboOrder order, DateTime now)
    {
        foreach (var item in order.Items)
        {
            if (!string.Equals(item.ItemType, "Car", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var car = context.Cars.SingleOrDefault(c => c.CarId == item.ReferenceId)
                ?? throw new InvalidOperationException($"Khong tim thay xe voi ID {item.ReferenceId}.");

            if (!string.Equals(car.Status, "Available", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Xe '{car.CarName}' hien khong san sang de dat coc.");
            }

            car.Status = "Reserved";
        }

        order.IsCaptchaUsed = true;
        order.CaptchaUsedAt = now;
        order.DepositExpiresAt = now.AddDays(DepositHoldDays);
        order.UpdatedAt = now;
        order.Status = "Deposited";
    }

    private static void CompleteBuyout(CarShowroomContext context, ComboOrder order, DateTime now)
    {
        foreach (var item in order.Items)
        {
            if (string.Equals(item.ItemType, "Car", StringComparison.OrdinalIgnoreCase))
            {
                var car = context.Cars.SingleOrDefault(c => c.CarId == item.ReferenceId)
                    ?? throw new InvalidOperationException($"Khong tim thay xe voi ID {item.ReferenceId}.");

                if (string.Equals(car.Status, "Sold", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Xe '{car.CarName}' da duoc ban.");
                }

                car.Status = "Sold";
            }
            else if (string.Equals(item.ItemType, "Part", StringComparison.OrdinalIgnoreCase))
            {
                var part = context.Parts.SingleOrDefault(p => p.PartId == item.ReferenceId)
                    ?? throw new InvalidOperationException($"Khong tim thay phu tung voi ID {item.ReferenceId}.");

                if (part.Quantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Phu tung '{part.PartName}' khong du ton kho de hoan tat don.");
                }

                part.Quantity -= item.Quantity;
                if (part.Quantity == 0)
                {
                    part.Status = "Out of Stock";
                }
            }
        }

        if (NormalizePurchaseType(order.PurchaseType) == "Deposit")
        {
            order.IsFinalCaptchaUsed = true;
            order.FinalCaptchaUsedAt = now;
        }
        else
        {
            order.IsCaptchaUsed = true;
            order.CaptchaUsedAt = now;
        }

        order.Status = "Completed";
        order.UpdatedAt = now;
    }

    private static void ReleaseReservedCars(CarShowroomContext context, ComboOrder order)
    {
        foreach (var item in order.Items)
        {
            if (!string.Equals(item.ItemType, "Car", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var car = context.Cars.SingleOrDefault(c => c.CarId == item.ReferenceId);
            if (car != null && string.Equals(car.Status, "Reserved", StringComparison.OrdinalIgnoreCase))
            {
                car.Status = "Available";
            }
        }
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
        } while (ExistsAnyCaptcha(context, 0, code));

        return code;
    }
}
