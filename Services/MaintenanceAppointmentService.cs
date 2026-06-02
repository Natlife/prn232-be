using System.Collections.Generic;
using BusinessObjects.Models;
using Repositories;

namespace Services
{
    public class MaintenanceAppointmentService : IMaintenanceAppointmentService
    {
        private readonly IMaintenanceAppointmentRepository _repository;

        public MaintenanceAppointmentService(IMaintenanceAppointmentRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<MaintenanceAppointment> GetAllAppointments() => _repository.GetAllAppointments();

        public IEnumerable<MaintenanceAppointment> GetAppointmentsByCustomerId(int customerId) => _repository.GetAppointmentsByCustomerId(customerId);

        public MaintenanceAppointment GetAppointmentById(int appointmentId) => _repository.GetAppointmentById(appointmentId);

        public void CreateAppointment(MaintenanceAppointment appointment)
        {
            appointment.Status = "Pending";
            _repository.AddAppointment(appointment);
        }

        public void UpdateAppointmentStatus(int appointmentId, string status)
        {
            var appointment = _repository.GetAppointmentById(appointmentId);
            if (appointment != null)
            {
                appointment.Status = status;
                _repository.UpdateAppointment(appointment);
            }
        }

        public void DeleteAppointment(int appointmentId) => _repository.DeleteAppointment(appointmentId);
    }
}
