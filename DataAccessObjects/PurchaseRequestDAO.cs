using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Common;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public static class PurchaseRequestDAO
{
    public static DepositResult CreateDeposit(DepositRequest req)
    {
        using var ctx = new CarShowroomContext();
        using var transaction = ctx.Database.BeginTransaction();
        try
        {
            var captcha = ctx.DepositCaptchas.SingleOrDefault(c => c.Code == req.CaptchaCode && c.CarId == req.CarId);
            if (captcha == null)
                return new DepositResult { Success = false, Message = "Mã xác nhận không tồn tại hoặc không hợp lệ cho xe này." };
            if (captcha.IsUsed)
                return new DepositResult { Success = false, Message = "Mã xác nhận này đã được sử dụng." };

            var car = ctx.Cars.SingleOrDefault(c => c.CarId == req.CarId);
            if (car == null)
                return new DepositResult { Success = false, Message = "Không tìm thấy xe." };
            if (car.Status != "Available")
                return new DepositResult { Success = false, Message = "Xe hiện không có sẵn (đã được đặt cọc hoặc đã bán)." };

            bool alreadyDeposited = ctx.PurchaseRequests.Any(p =>
                p.CarId == req.CarId &&
                p.CustomerId == req.CustomerId &&
                p.Status == "Deposited" &&
                p.DepositExpiry > DateTime.Now);
            if (alreadyDeposited)
                return new DepositResult { Success = false, Message = "Bạn đã có lượt đặt cọc xe này đang hoạt động." };

            decimal depositAmount = Math.Round(car.Price * 0.05m, 0);
            var now = DateTime.Now;

            var request = new PurchaseRequest
            {
                CarId = req.CarId,
                CustomerId = req.CustomerId,
                CustomerName = req.CustomerName,
                CustomerPhone = req.CustomerPhone,
                CustomerEmail = req.CustomerEmail,
                Status = "Deposited",
                CreatedAt = now,
                DepositAmount = depositAmount,
                DepositDate = now,
                DepositExpiry = now.AddDays(14),
                CaptchaCode = req.CaptchaCode,
                Message = $"Đặt cọc xe. Mã xác nhận: {req.CaptchaCode}"
            };
            ctx.PurchaseRequests.Add(request);

            car.Status = "Reserved";

            captcha.IsUsed = true;
            captcha.UsedAt = now;

            ctx.SaveChanges();
            transaction.Commit();

            return new DepositResult
            {
                Success = true,
                Message = "Đặt cọc thành công!",
                RequestId = request.RequestId,
                DepositAmount = depositAmount,
                DepositExpiry = request.DepositExpiry
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new DepositResult { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" };
        }
    }

    public static IEnumerable<PurchaseRequest> GetDepositsByCustomer(int customerId)
    {
        using var ctx = new CarShowroomContext();
        return ctx.PurchaseRequests
            .Include(p => p.Car)
            .Where(p => p.CustomerId == customerId && p.DepositExpiry != null)
            .ToList();
    }
}
