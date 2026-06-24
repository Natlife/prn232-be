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

            if (car.Status != "Available")
            {
                return new DepositResult { Success = false, Message = "Xe hien khong co san." };
            }

            var alreadyDeposited = ctx.PurchaseRequests.Any(p =>
                p.CarId == req.CarId &&
                p.CustomerId == req.CustomerId &&
                p.Status == "Pending" &&
                p.DepositExpiry > DateTime.Now);
            if (alreadyDeposited)
                return new DepositResult { Success = false, Message = "Bạn đã có lượt đặt cọc cho xe này đang hoạt động." };

            if (alreadyDeposited)
            {
                return new DepositResult { Success = false, Message = "Bạn đã có lượt đặt cọc cho xe này đang hoạt động." };
            }

            var depositAmount = Math.Round(car.Price * 0.05m, 0);
            var now = DateTime.Now;

            var request = new PurchaseRequest
            {
                CarId = req.CarId,
                CustomerId = req.CustomerId,
                CustomerName = req.CustomerName,
                CustomerPhone = req.CustomerPhone,
                CustomerEmail = req.CustomerEmail,
                Status = "Pending",
                CreatedAt = now,
                DepositAmount = depositAmount,
                DepositDate = now,
                DepositExpiry = now.AddDays(14),
                CaptchaCode = req.CaptchaCode,
                Message = $"Đã đặt cọc: {req.CaptchaCode}"
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

    public static DepositResult CreateBuyout(DepositRequest req)
    {
        using var ctx = new CarShowroomContext();
        using var transaction = ctx.Database.BeginTransaction();

        try
        {
            var car = ctx.Cars.SingleOrDefault(c => c.CarId == req.CarId);
            if (car == null)
                return new DepositResult { Success = false, Message = "Không tìm thấy xe." };

            var now = DateTime.Now;
            decimal paymentAmount;
            PurchaseRequest request;

            if (car.Status == "Reserved")
            {
                var activeDeposit = ctx.PurchaseRequests.SingleOrDefault(p =>
                    p.CarId == req.CarId &&
                    p.CustomerId == req.CustomerId &&
                    p.Status == "Pending" &&
                    p.DepositExpiry != null &&
                    p.DepositExpiry > now);

                if (activeDeposit == null)
                    return new DepositResult { Success = false, Message = "Xe khong co trong danh sach dat coc." };

                if (false && !string.Equals(activeDeposit.CaptchaCode, req.CaptchaCode, StringComparison.OrdinalIgnoreCase))
                    return new DepositResult { Success = false, Message = "Mã xác nhận không đúng." };

                var paidDeposit = activeDeposit.DepositAmount ?? Math.Round(car.Price * 0.05m, 0);
                paymentAmount = car.Price - paidDeposit;

                activeDeposit.Status = "Completed";
                activeDeposit.UpdatedAt = now;
                activeDeposit.Message = $"Hoàn tất mua xe, địa chỉ: {paidDeposit:N0}. còn lại: {paymentAmount:N0}. Mã xác nhận: {req.CaptchaCode}";
                activeDeposit.DepositAmount = paymentAmount;
                activeDeposit.DepositExpiry = null;
                request = activeDeposit;
            }
            else
            {
                if (car.Status != "Available")
                    return new DepositResult { Success = false, Message = "Xe hiện không còn" };

                var captcha = ctx.DepositCaptchas.SingleOrDefault(c => c.Code == req.CaptchaCode && c.CarId == req.CarId);
                if (captcha == null)
                    return new DepositResult { Success = false, Message = "Mã xác nhận không tồn tại" };
                if (captcha.IsUsed)
                    return new DepositResult { Success = false, Message = "Mã xác nhận này đã được sử dụng." };

                paymentAmount = car.Price;
                request = new PurchaseRequest
                {
                    CarId = req.CarId,
                    CustomerId = req.CustomerId,
                    CustomerName = req.CustomerName,
                    CustomerPhone = req.CustomerPhone,
                    CustomerEmail = req.CustomerEmail,
                    Status = "Completed",
                    CreatedAt = now,
                    DepositAmount = paymentAmount,
                    DepositDate = now,
                    DepositExpiry = null,
                    CaptchaCode = req.CaptchaCode,
                    Message = $"Mua đứt xe, mã: {req.CaptchaCode}"
                };

                ctx.PurchaseRequests.Add(request);
                captcha.IsUsed = true;
                captcha.UsedAt = now;
            }

            car.Status = "Sold";

            ctx.SaveChanges();
            transaction.Commit();

            return new DepositResult
            {
                Success = true,
                Message = "Mua đứt xe thành công!",
                RequestId = request.RequestId,
                DepositAmount = paymentAmount,
                DepositExpiry = null
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
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }

    public static IEnumerable<PurchaseRequest> GetAllPurchaseRequests()
    {
        using var ctx = new CarShowroomContext();
        return ctx.PurchaseRequests
            .Include(p => p.Car)
            .ToList();
    }
}
