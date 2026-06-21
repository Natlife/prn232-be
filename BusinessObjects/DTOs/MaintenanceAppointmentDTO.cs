using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTOs
{
    public class MaintenanceAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int PackageId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = null!;
        
        [Required]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = null!;
        
        [StringLength(100)]
        public string? CustomerEmail { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CarName { get; set; } = null!;
        
        [StringLength(20)]
        public string? LicensePlate { get; set; }
        
        [Required]
        public DateOnly AppointmentDate { get; set; }
        
        [Required]
        public TimeOnly AppointmentTime { get; set; }
        
        [StringLength(1000)]
        public string? Note { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = null!;
        
        public DateTime? CreatedAt { get; set; }
        
        // Navigation property serialized as DTO
        public MaintenancePackageDTO? Package { get; set; }
    }
}
