using System.Collections.Generic;
using BusinessObjects.Models;

namespace Services
{
    public interface IComboOrderService
    {
        IEnumerable<ComboOrder> GetAllOrders();
        ComboOrder? GetOrderById(int orderId);
        IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId);
        void AddOrder(ComboOrder order);
        void UpdateOrder(ComboOrder order);
        void GenerateCaptcha(int orderId, string? customCode);
        bool VerifyCaptcha(int orderId, string captchaCode);
        void CancelOrder(int orderId);
    }
}
