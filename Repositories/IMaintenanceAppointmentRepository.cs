using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories
{
    public interface IMaintenanceAppointmentRepository
    {
        IEnumerable<MaintenanceAppointment> GetAllAppointments();
        IEnumerable<MaintenanceAppointment> GetAppointmentsByCustomerId(int customerId);
        MaintenanceAppointment GetAppointmentById(int appointmentId);
        void AddAppointment(MaintenanceAppointment appointment);
        void UpdateAppointment(MaintenanceAppointment appointment);
        void DeleteAppointment(int appointmentId);
    }
}
