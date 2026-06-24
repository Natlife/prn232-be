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
                return new DepositResult { Success = false, Message = "M? x?c nh?n kh?ng t?n t?i ho?c kh?ng h?p l? cho xe n?y." };
            if (captcha.IsUsed)
                return new DepositResult { Success = false, Message = "M? x?c nh?n n?y d? du?c s? d?ng." };

            var car = ctx.Cars.SingleOrDefault(c => c.CarId == req.CarId);
            if (car == null)
                return new DepositResult { Success = false, Message = "Kh?ng t?m th?y xe." };
            if (car.Status != "Available")
                return new DepositResult { Success = false, Message = "Xe hi?n kh?ng c? s?n (d? du?c d?t c?c ho?c d? b?n)." };

            bool alreadyDeposited = ctx.PurchaseRequests.Any(p =>
                p.CarId == req.CarId &&
                p.CustomerId == req.CustomerId &&
                p.Status == "Pending" &&
                p.DepositExpiry > DateTime.Now);
            if (alreadyDeposited)
                return new DepositResult { Success = false, Message = "B?n d? c? lu?t d?t c?c xe n?y dang ho?t d?ng." };

            decimal depositAmount = Math.Round(car.Price * 0.05m, 0);
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
                Message = $"D?t c?c xe. M? x?c nh?n: {req.CaptchaCode}"
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
                Message = "D?t c?c th?nh c?ng!",
                RequestId = request.RequestId,
                DepositAmount = depositAmount,
                DepositExpiry = request.DepositExpiry
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new DepositResult { Success = false, Message = $"L?i h? th?ng: {ex.Message}" };
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
                return new DepositResult { Success = false, Message = "Kh?ng t?m th?y xe." };

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
                    return new DepositResult { Success = false, Message = "Xe d? du?c d?t c?c. Ch? ngu?i d?t c?c hi?n t?i m?i c? th? mua d?t ph?n c?n l?i." };

                if (false && !string.Equals(activeDeposit.CaptchaCode, req.CaptchaCode, StringComparison.OrdinalIgnoreCase))
                    return new DepositResult { Success = false, Message = "M? x?c nh?n kh?ng kh?p v?i lu?t d?t c?c hi?n t?i." };

                var paidDeposit = activeDeposit.DepositAmount ?? Math.Round(car.Price * 0.05m, 0);
                paymentAmount = car.Price - paidDeposit;

                activeDeposit.Status = "Completed";
                activeDeposit.UpdatedAt = now;
                activeDeposit.Message = $"Ho?n t?t mua d?t xe. D? c?c: {paidDeposit:N0}. C?n l?i: {paymentAmount:N0}. M? x?c nh?n: {req.CaptchaCode}";
                activeDeposit.DepositAmount = paymentAmount;
                activeDeposit.DepositExpiry = null;
                request = activeDeposit;
            }
            else
            {
                if (car.Status != "Available")
                    return new DepositResult { Success = false, Message = "Xe hi?n kh?ng c? s?n (d? du?c d?t c?c ho?c d? b?n)." };

                var captcha = ctx.DepositCaptchas.SingleOrDefault(c => c.Code == req.CaptchaCode && c.CarId == req.CarId);
                if (captcha == null)
                    return new DepositResult { Success = false, Message = "M? x?c nh?n kh?ng t?n t?i ho?c kh?ng h?p l? cho xe n?y." };
                if (captcha.IsUsed)
                    return new DepositResult { Success = false, Message = "M? x?c nh?n n?y d? du?c s? d?ng." };

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
                    Message = $"Mua d?t xe. M? x?c nh?n: {req.CaptchaCode}"
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
                Message = "Mua d?t xe th?nh c?ng!",
                RequestId = request.RequestId,
                DepositAmount = paymentAmount,
                DepositExpiry = null
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new DepositResult { Success = false, Message = $"L?i h? th?ng: {ex.Message}" };
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
