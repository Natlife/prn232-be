using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Common;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

/// <summary>
/// Yêu cầu mua xe của khách hàng (module ô tô).
/// Việc phát hành hóa đơn + đặt cọc/mua đứt + xác thực captcha được xử lý ở tầng dùng chung
/// (CheckoutDAO tạo hóa đơn tổng, MasterInvoicePaymentDAO xử lý thanh toán) để thống nhất cho cả 3 module.
/// Singleton pattern theo backend_conventions.md.
/// </summary>
public class CarSalesDAO
{
    private static CarSalesDAO? _instance;
    private static readonly object _lock = new();
    private CarSalesDAO() { }

    public static CarSalesDAO Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new CarSalesDAO();
                return _instance;
            }
        }
    }

    public ServiceResult CreatePurchaseRequest(CreatePurchaseRequestDto dto, int customerId)
    {
        using var ctx = new CarShowroomContext();

        var car = ctx.Cars.SingleOrDefault(c => c.CarId == dto.CarId);
        if (car == null) return ServiceResult.Fail("Không tìm thấy xe.");
        if (car.Status is "Sold" or "Inactive") return ServiceResult.Fail("Xe này hiện không còn được bán.");

        var existing = ctx.PurchaseRequests
            .Where(p => p.CarId == dto.CarId && p.CustomerId == customerId &&
                        (p.Status == "Pending" || p.Status == "Confirmed"))
            .OrderByDescending(p => p.RequestId)
            .FirstOrDefault();
        if (existing != null)
        {
            // Đã có hóa đơn đang chờ thanh toán -> chặn để tránh mua trùng.
            if (existing.Status == "Confirmed")
                return ServiceResult.Fail("Xe này đã có hóa đơn đang chờ thanh toán. Vui lòng vào trang 'Hóa đơn của tôi'.");
            // Yêu cầu còn Pending (chưa lập hóa đơn) -> dùng lại (idempotent) để checkout thử lại được.
            return ServiceResult.Ok("Dùng lại yêu cầu mua đang chờ cho xe này.", new { requestId = existing.RequestId });
        }

        var now = DateTime.Now;
        var request = new PurchaseRequest
        {
            CarId = dto.CarId,
            CustomerId = customerId,
            CustomerName = dto.CustomerName.Trim(),
            CustomerPhone = dto.CustomerPhone.Trim(),
            CustomerEmail = dto.CustomerEmail?.Trim(),
            Message = dto.Message?.Trim(),
            Status = "Pending",
            CreatedAt = now,
            CreatedUser = customerId
        };
        ctx.PurchaseRequests.Add(request);
        ctx.SaveChanges();

        return ServiceResult.Ok(
            "Đã gửi yêu cầu mua xe. Vui lòng liên hệ nhân viên để được lập hóa đơn và nhận mã xác thực.",
            new { requestId = request.RequestId });
    }

    public IEnumerable<PurchaseRequest> GetPurchaseRequests(int? customerId)
    {
        using var ctx = new CarShowroomContext();
        IQueryable<PurchaseRequest> query = ctx.PurchaseRequests.AsNoTracking().Include(p => p.Car);
        if (customerId.HasValue) query = query.Where(p => p.CustomerId == customerId.Value);
        return query.OrderByDescending(p => p.CreatedAt).ToList();
    }
}
