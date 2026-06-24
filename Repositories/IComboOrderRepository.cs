using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories;

public interface IComboOrderRepository
{
    IEnumerable<ComboOrder> GetAllOrders();
    IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId);
    ComboOrder? GetOrderById(int comboOrderId);
    void AddOrder(ComboOrder order);
    void UpdateStatus(int comboOrderId, string newStatus);
    ComboOrder GenerateCaptcha(int comboOrderId, string? code);
    ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode);
}
