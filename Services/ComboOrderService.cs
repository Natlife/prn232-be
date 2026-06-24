using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using DataAccessObjects;
using Repositories;

namespace Services;

public class ComboOrderService : IComboOrderService
{
    private static readonly HashSet<string> AllowedItemTypes = new() { "Car", "Part", "Service" };
    private static readonly HashSet<string> AllowedPurchaseTypes = new() { "Deposit", "Buyout" };

    private readonly IComboOrderRepository _comboOrderRepository;

    public ComboOrderService(IComboOrderRepository comboOrderRepository)
    {
        _comboOrderRepository = comboOrderRepository;
    }

    public ComboOrderPreviewDto PreviewDraft(List<ComboOrderItemInputDto> draftItems)
    {
        var previews = draftItems.Select(ResolveItemPreview).ToList();
        return new ComboOrderPreviewDto
        {
            Items = previews,
            TotalAmount = previews.Sum(p => p.SubTotal),
            DraftToken = string.Empty
        };
    }

    public ComboOrder PlaceOrder(ComboOrderCreateDto dto, int customerId, string customerName, string source = "manual")
    {
        var resolvedItems = dto.Items.Select(ResolveItemPreview).ToList();
        var purchaseType = string.IsNullOrWhiteSpace(dto.PurchaseType) ? "Buyout" : dto.PurchaseType.Trim();
        if (!AllowedPurchaseTypes.Contains(purchaseType))
        {
            throw new InvalidOperationException("Loai giao dich khong hop le. Chi chap nhan Deposit hoac Buyout.");
        }

        var totalAmount = resolvedItems.Sum(i => i.SubTotal);
        var depositAmount = purchaseType == "Deposit"
            ? Math.Round(totalAmount * 0.05m, 0)
            : totalAmount;

        var order = new ComboOrder
        {
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerPhone = dto.CustomerPhone,
            ShippingAddress = dto.ShippingAddress,
            Note = dto.Note,
            ChatSessionId = dto.ChatSessionId,
            Source = source,
            PurchaseType = purchaseType,
            TotalAmount = totalAmount,
            DepositAmount = depositAmount,
            Items = resolvedItems.Select(i => new ComboOrderItem
            {
                ItemType = i.ItemType,
                ReferenceId = i.ReferenceId,
                ItemName = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        };

        _comboOrderRepository.AddOrder(order);
        return order;
    }

    public ComboOrder? GetOrderById(int comboOrderId) => _comboOrderRepository.GetOrderById(comboOrderId);

    public IEnumerable<ComboOrder> GetAllOrders() => _comboOrderRepository.GetAllOrders();

    public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId) => _comboOrderRepository.GetOrdersByCustomerId(customerId);

    public void UpdateStatus(int comboOrderId, string newStatus) => _comboOrderRepository.UpdateStatus(comboOrderId, newStatus);

    public ComboOrder GenerateCaptcha(int comboOrderId, string? code) => _comboOrderRepository.GenerateCaptcha(comboOrderId, code);

    public ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode) => _comboOrderRepository.VerifyCaptcha(comboOrderId, customerId, captchaCode);

    private static ComboOrderItemPreviewDto ResolveItemPreview(ComboOrderItemInputDto input)
    {
        if (!AllowedItemTypes.Contains(input.ItemType))
        {
            throw new InvalidOperationException($"ItemType khong hop le: '{input.ItemType}'. Chi chap nhan Car, Part, Service.");
        }

        return input.ItemType switch
        {
            "Car" => ResolveCarItem(input),
            "Part" => ResolvePartItem(input),
            "Service" => ResolveServiceItem(input),
            _ => throw new InvalidOperationException($"Unhandled ItemType: {input.ItemType}")
        };
    }

    private static ComboOrderItemPreviewDto ResolveCarItem(ComboOrderItemInputDto input)
    {
        using var context = new CarShowroomContext();
        var car = context.Cars.SingleOrDefault(c => c.CarId == input.ReferenceId);

        if (car is null)
        {
            throw new InvalidOperationException($"Khong tim thay xe voi ID {input.ReferenceId}.");
        }

        if (car.Status == "Inactive")
        {
            throw new InvalidOperationException($"Xe '{car.CarName}' hien khong con kinh doanh.");
        }

        return BuildPreview(input, "Car", car.CarName, car.Price, car.ImageUrl);
    }

    private static ComboOrderItemPreviewDto ResolvePartItem(ComboOrderItemInputDto input)
    {
        using var context = new CarShowroomContext();
        var part = context.Parts.SingleOrDefault(p => p.PartId == input.ReferenceId);

        if (part is null)
        {
            throw new InvalidOperationException($"Khong tim thay phu tung voi ID {input.ReferenceId}.");
        }

        if (part.Status == "Inactive")
        {
            throw new InvalidOperationException($"Phu tung '{part.PartName}' hien khong con kinh doanh.");
        }

        if (part.Quantity < input.Quantity)
        {
            throw new InvalidOperationException($"Ton kho phu tung '{part.PartName}' khong du (con {part.Quantity}).");
        }

        return BuildPreview(input, "Part", part.PartName, part.Price, part.ImageUrl);
    }

    private static ComboOrderItemPreviewDto ResolveServiceItem(ComboOrderItemInputDto input)
    {
        using var context = new CarShowroomContext();
        var pkg = context.MaintenancePackages.SingleOrDefault(p => p.PackageId == input.ReferenceId);

        if (pkg is null)
        {
            throw new InvalidOperationException($"Khong tim thay dich vu voi ID {input.ReferenceId}.");
        }

        if (pkg.Status == "Inactive")
        {
            throw new InvalidOperationException($"Dich vu '{pkg.PackageName}' hien tam ngung.");
        }

        return BuildPreview(input, "Service", pkg.PackageName, pkg.Price, null);
    }

    private static ComboOrderItemPreviewDto BuildPreview(
        ComboOrderItemInputDto input,
        string resolvedType,
        string name,
        decimal unitPrice,
        string? imageUrl)
    {
        return new ComboOrderItemPreviewDto
        {
            ItemType = resolvedType,
            ReferenceId = input.ReferenceId,
            Name = name,
            UnitPrice = unitPrice,
            Quantity = input.Quantity,
            SubTotal = unitPrice * input.Quantity,
            ImageUrl = imageUrl
        };
    }
}
