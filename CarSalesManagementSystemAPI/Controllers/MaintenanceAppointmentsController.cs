using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BusinessObjects.Models;
using Services;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceAppointmentsController : ControllerBase
    {
        private readonly IMaintenanceAppointmentService _service;

        public MaintenanceAppointmentsController(IMaintenanceAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MaintenanceAppointment>> Get()
        {
            return Ok(_service.GetAllAppointments());
        }

        [HttpGet("customer/{customerId}")]
        public ActionResult<IEnumerable<MaintenanceAppointment>> GetByCustomer(int customerId)
        {
            return Ok(_service.GetAppointmentsByCustomerId(customerId));
        }

        [HttpGet("{id}")]
        public ActionResult<MaintenanceAppointment> Get(int id)
        {
            var appointment = _service.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpPost]
        public IActionResult Post([FromBody] MaintenanceAppointment appointment)
        {
            _service.CreateAppointment(appointment);
            return CreatedAtAction(nameof(Get), new { id = appointment.AppointmentId }, appointment);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] string status)
        {
            _service.UpdateAppointmentStatus(id, status);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.DeleteAppointment(id);
            return NoContent();
        }
    }
}
