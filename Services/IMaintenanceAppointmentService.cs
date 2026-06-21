using System.Collections.Generic;
using BusinessObjects.Models;

namespace Services
{
    public interface IMaintenanceAppointmentService
    {
        IEnumerable<MaintenanceAppointment> GetAllAppointments();
        IEnumerable<MaintenanceAppointment> GetAppointmentsByCustomerId(int customerId);
        MaintenanceAppointment GetAppointmentById(int appointmentId);
        void CreateAppointment(MaintenanceAppointment appointment);
        void UpdateAppointmentStatus(int appointmentId, string status, string? reason = null);
        void DeleteAppointment(int appointmentId);
    }
}
