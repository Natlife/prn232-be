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
            {
                return new DepositResult { Success = false, Message = "Ma xac nhan khong ton tai hoac khong hop le cho xe nay." };
            }

            if (captcha.IsUsed)
            {
                return new DepositResult { Success = false, Message = "Ma xac nhan nay da duoc su dung." };
            }

            var car = ctx.Cars.SingleOrDefault(c => c.CarId == req.CarId);
            if (car == null)
            {
                return new DepositResult { Success = false, Message = "Khong tim thay xe." };
            }

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
            {
                return new DepositResult { Success = false, Message = "Ban da co luot dat coc xe nay dang hoat dong." };
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
                Message = $"Dat coc xe. Ma xac nhan: {req.CaptchaCode}"
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
                Message = "Dat coc thanh cong!",
                RequestId = request.RequestId,
                DepositAmount = depositAmount,
                DepositExpiry = request.DepositExpiry
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new DepositResult { Success = false, Message = $"Loi he thong: {ExceptionMessageHelper.GetDetailedMessage(ex)}" };
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
            {
                return new DepositResult { Success = false, Message = "Khong tim thay xe." };
            }

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
                {
                    return new DepositResult { Success = false, Message = "Xe da duoc dat coc boi khach hang khac." };
                }

                var paidDeposit = activeDeposit.DepositAmount ?? Math.Round(car.Price * 0.05m, 0);
                paymentAmount = car.Price - paidDeposit;

                activeDeposit.Status = "Completed";
                activeDeposit.UpdatedAt = now;
                activeDeposit.Message = $"Hoan tat mua dut xe. Da coc: {paidDeposit:N0}. Con lai: {paymentAmount:N0}. Ma xac nhan: {req.CaptchaCode}";
                activeDeposit.DepositAmount = paymentAmount;
                activeDeposit.DepositExpiry = null;
                request = activeDeposit;
            }
            else
            {
                if (car.Status != "Available")
                {
                    return new DepositResult { Success = false, Message = "Xe hien khong co san." };
                }

                var captcha = ctx.DepositCaptchas.SingleOrDefault(c => c.Code == req.CaptchaCode && c.CarId == req.CarId);
                if (captcha == null)
                {
                    return new DepositResult { Success = false, Message = "Ma xac nhan khong ton tai hoac khong hop le cho xe nay." };
                }

                if (captcha.IsUsed)
                {
                    return new DepositResult { Success = false, Message = "Ma xac nhan nay da duoc su dung." };
                }

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
                    Message = $"Mua dut xe. Ma xac nhan: {req.CaptchaCode}"
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
                Message = "Mua dut xe thanh cong!",
                RequestId = request.RequestId,
                DepositAmount = paymentAmount,
                DepositExpiry = null
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new DepositResult { Success = false, Message = $"Loi he thong: {ExceptionMessageHelper.GetDetailedMessage(ex)}" };
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
