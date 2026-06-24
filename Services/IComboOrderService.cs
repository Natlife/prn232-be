using System.Collections.Generic;
using BusinessObjects.DTOs;
using BusinessObjects.Models;

namespace Services;

public interface IComboOrderService
{
    /// <summary>
    /// Resolves names and server-side prices for each draft item.
    /// Throws InvalidOperationException if any item is not found or unavailable.
    /// </summary>
    ComboOrderPreviewDto PreviewDraft(List<ComboOrderItemInputDto> draftItems);

    /// <summary>
    /// Creates and persists a ComboOrder. Prices are always re-resolved from DB
    /// — client-supplied prices are ignored.
    /// </summary>
    ComboOrder PlaceOrder(ComboOrderCreateDto dto, int customerId, string customerName, string source = "manual");

    ComboOrder? GetOrderById(int comboOrderId);
    IEnumerable<ComboOrder> GetAllOrders();
    IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId);

    /// <summary>Admin-only status transition.</summary>
    void UpdateStatus(int comboOrderId, string newStatus);
    ComboOrder GenerateCaptcha(int comboOrderId, string? code);
    ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode);
}
