using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

// ─── FLOW ─────────────────────────────────────────────────────────────────────
//
//  Singleton pattern — consistent with existing DAOs in this codebase.
//  Each public method opens a scoped CarShowroomContext via `using`
//  to ensure connection is returned to pool immediately after use.
//
//  GetById / GetByCustomer → Include Items for full graph
//  AddOrder               → sets server-side fields (CreatedAt, Status)
//  UpdateStatus           → targeted update — only touches Status + UpdatedAt
//
// ─────────────────────────────────────────────────────────────────────────────

public class ComboOrderDAO
{
    private static ComboOrderDAO? _instance;
    private static readonly object _lock = new();

    private ComboOrderDAO() { }

    public static ComboOrderDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new ComboOrderDAO();
            }
        }
    }

    public IEnumerable<ComboOrder> GetAllOrders()
    {
        using var context = new CarShowroomContext();
        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId)
    {
        using var context = new CarShowroomContext();
        return context.ComboOrders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    public ComboOrder? GetOrderById(int comboOrderId)
    {
        using var context = new CarShowroomContext();
        return context.ComboOrders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .SingleOrDefault(o => o.ComboOrderId == comboOrderId);
    }

    public void AddOrder(ComboOrder order)
    {
        using var context = new CarShowroomContext();
        order.CreatedAt = DateTime.Now;
        order.Status = "Pending";
        context.ComboOrders.Add(order);
        context.SaveChanges();
    }

    public void UpdateStatus(int comboOrderId, string newStatus)
    {
        using var context = new CarShowroomContext();
        var order = context.ComboOrders.SingleOrDefault(o => o.ComboOrderId == comboOrderId);
        if (order is null)
            throw new InvalidOperationException($"ComboOrder {comboOrderId} not found.");

        order.Status = newStatus;
        order.UpdatedAt = DateTime.Now;
        context.Entry(order).State = EntityState.Modified;
        context.SaveChanges();
    }
}
