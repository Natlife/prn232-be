using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class ComboOrderDAO
    {
        private static ComboOrderDAO instance = null;
        private static readonly object instanceLock = new object();

        private ComboOrderDAO() { }

        public static ComboOrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ComboOrderDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<ComboOrder> GetAllOrders()
        {
            using var context = new CarShowroomContext();
            return context.ComboOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .ToList();
        }

        public ComboOrder? GetOrderById(int orderId)
        {
            using var context = new CarShowroomContext();
            return context.ComboOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .SingleOrDefault(o => o.ComboOrderId == orderId);
        }

        public IEnumerable<ComboOrder> GetOrdersByCustomerId(int customerId)
        {
            using var context = new CarShowroomContext();
            return context.ComboOrders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .ToList();
        }

        public void AddOrder(ComboOrder order)
        {
            using var context = new CarShowroomContext();
            context.ComboOrders.Add(order);
            context.SaveChanges();
        }

        public void UpdateOrder(ComboOrder order)
        {
            using var context = new CarShowroomContext();
            context.Entry(order).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
