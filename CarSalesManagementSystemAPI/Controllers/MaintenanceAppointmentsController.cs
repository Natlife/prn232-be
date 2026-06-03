using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using BusinessObjects.DTOs;
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

        private MaintenanceAppointmentDTO MapToDTO(MaintenanceAppointment a)
        {
            return new MaintenanceAppointmentDTO 
            {
                AppointmentId = a.AppointmentId,
                CustomerId = a.CustomerId,
                PackageId = a.PackageId,
                CustomerName = a.CustomerName,
                CustomerPhone = a.CustomerPhone,
                CustomerEmail = a.CustomerEmail,
                CarName = a.CarName,
                LicensePlate = a.LicensePlate,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Note = a.Note,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Package = a.Package != null ? new MaintenancePackageDTO 
                {
                    PackageId = a.Package.PackageId,
                    PackageName = a.Package.PackageName,
                    Description = a.Package.Description,
                    Price = a.Package.Price,
                    EstimatedDuration = a.Package.EstimatedDuration,
                    Status = a.Package.Status
                } : null
            };
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<MaintenanceAppointmentDTO>>> Get()
        {
            var data = _service.GetAllAppointments().Select(MapToDTO).ToList();
            return Ok(new ApiResponse<IEnumerable<MaintenanceAppointmentDTO>>(true, "Success", data));
        }

        [HttpGet("customer/{customerId}")]
        public ActionResult<ApiResponse<IEnumerable<MaintenanceAppointmentDTO>>> GetByCustomer(int customerId)
        {
            var data = _service.GetAppointmentsByCustomerId(customerId).Select(MapToDTO).ToList();
            return Ok(new ApiResponse<IEnumerable<MaintenanceAppointmentDTO>>(true, "Success", data));
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<MaintenanceAppointmentDTO>> Get(int id)
        {
            var appointment = _service.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound(new ApiResponse<MaintenanceAppointmentDTO>(false, "Không tìm thấy"));
            }
            return Ok(new ApiResponse<MaintenanceAppointmentDTO>(true, "Success", MapToDTO(appointment)));
        }

        [HttpPost]
        public ActionResult<ApiResponse<MaintenanceAppointmentDTO>> Post([FromBody] MaintenanceAppointmentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiResponse<object>(false, "Dữ liệu không hợp lệ", ModelState));

            var appointment = new MaintenanceAppointment
            {
                CustomerId = dto.CustomerId,
                PackageId = dto.PackageId,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                CarName = dto.CarName,
                LicensePlate = dto.LicensePlate,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Note = dto.Note,
                Status = dto.Status ?? "Pending"
            };

            _service.CreateAppointment(appointment);
            
            return Ok(new ApiResponse<MaintenanceAppointmentDTO>(true, "Thêm thành công", MapToDTO(appointment)));
        }

        [HttpPut("{id}/status")]
        public ActionResult<ApiResponse<string>> UpdateStatus(int id, [FromBody] string status)
        {
            _service.UpdateAppointmentStatus(id, status);
            return Ok(new ApiResponse<string>(true, "Cập nhật thành công"));
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<string>> Delete(int id)
        {
            _service.DeleteAppointment(id);
            return Ok(new ApiResponse<string>(true, "Xóa thành công"));
        }
    }
}
