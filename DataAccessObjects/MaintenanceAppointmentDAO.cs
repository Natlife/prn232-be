using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class MaintenanceAppointmentDAO
    {
        private static MaintenanceAppointmentDAO instance = null;
        private static readonly object instanceLock = new object();

        private MaintenanceAppointmentDAO() { }

        public static MaintenanceAppointmentDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MaintenanceAppointmentDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<MaintenanceAppointment> GetAllAppointments()
        {
            using var context = new CarShowroomContext();
            return context.MaintenanceAppointments
                .Include(a => a.Package)
                .Include(a => a.Customer)
                .ToList();
        }

        public IEnumerable<MaintenanceAppointment> GetAppointmentsByCustomerId(int customerId)
        {
            using var context = new CarShowroomContext();
            return context.MaintenanceAppointments
                .Include(a => a.Package)
                .Where(a => a.CustomerId == customerId)
                .ToList();
        }

        public MaintenanceAppointment GetAppointmentById(int appointmentId)
        {
            using var context = new CarShowroomContext();
            return context.MaintenanceAppointments
                .Include(a => a.Package)
                .Include(a => a.Customer)
                .SingleOrDefault(a => a.AppointmentId == appointmentId);
        }

        public void AddAppointment(MaintenanceAppointment appointment)
        {
            using var context = new CarShowroomContext();
            context.MaintenanceAppointments.Add(appointment);
            context.SaveChanges();
        }

        public void UpdateAppointment(MaintenanceAppointment appointment)
        {
            using var context = new CarShowroomContext();
            context.Entry(appointment).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void DeleteAppointment(int appointmentId)
        {
            using var context = new CarShowroomContext();
            var appointment = context.MaintenanceAppointments.SingleOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment != null)
            {
                context.MaintenanceAppointments.Remove(appointment);
                context.SaveChanges();
            }
        }
    }
}
