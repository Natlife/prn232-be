using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;

namespace Repositories
{
    public class ComboOrderRepository : IComboOrderRepository
    {
        public IEnumerable<ComboOrder> GetAllOrders() => ComboOrderDAO.Instance.GetAllOrders();

        public ComboOrder? GetOrderById(int orderId) => ComboOrderDAO.Instance.GetOrderById(orderId);

        public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId) => ComboOrderDAO.Instance.GetOrdersByCustomerId(customerId);

        public void AddOrder(ComboOrder order) => ComboOrderDAO.Instance.AddOrder(order);

        public void UpdateOrder(ComboOrder order) => ComboOrderDAO.Instance.UpdateOrder(order);
    }
}
