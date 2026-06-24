using System.Collections.Generic;
using BusinessObjects.DTOs;
using BusinessObjects.Models;

namespace Services;

public interface IComboOrderService
{
    ComboOrderPreviewDto PreviewDraft(List<ComboOrderItemInputDto> draftItems);

    ComboOrder PlaceOrder(ComboOrderCreateDto dto, int customerId, string customerName, string source = "manual");

    ComboOrder? GetOrderById(int comboOrderId);

    IEnumerable<ComboOrder> GetAllOrders();

    IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId);

    void UpdateStatus(int comboOrderId, string newStatus);

    ComboOrder GenerateCaptcha(int comboOrderId, string? code);

    ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode);
}
