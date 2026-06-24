using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;

namespace Repositories;

public class ComboOrderRepository : IComboOrderRepository
{
    public IEnumerable<ComboOrder> GetAllOrders() => ComboOrderDAO.Instance.GetAllOrders();

    public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId) => ComboOrderDAO.Instance.GetOrdersByCustomerId(customerId);

    public ComboOrder? GetOrderById(int comboOrderId) => ComboOrderDAO.Instance.GetOrderById(comboOrderId);

    public void AddOrder(ComboOrder order) => ComboOrderDAO.Instance.AddOrder(order);

    public void UpdateStatus(int comboOrderId, string newStatus) => ComboOrderDAO.Instance.UpdateStatus(comboOrderId, newStatus);

    public ComboOrder GenerateCaptcha(int comboOrderId, string? code) => ComboOrderDAO.Instance.GenerateCaptcha(comboOrderId, code);

    public ComboOrder VerifyCaptcha(int comboOrderId, int customerId, string captchaCode) => ComboOrderDAO.Instance.VerifyCaptcha(comboOrderId, customerId, captchaCode);
}
