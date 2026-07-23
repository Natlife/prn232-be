using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories
{
    public interface IComboOrderRepository
    {
        IEnumerable<ComboOrder> GetAllOrders();
        ComboOrder? GetOrderById(int orderId);
        IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId);
        void AddOrder(ComboOrder order);
        void UpdateOrder(ComboOrder order);
    }
}
