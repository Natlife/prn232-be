using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTOs
{
    public class MaintenancePackageDTO
    {
        public int PackageId { get; set; }

        [Required(ErrorMessage = "Tên gói không được trống")]
        [StringLength(150, ErrorMessage = "Tên gói không quá 150 ký tự")]
        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Giá không hợp lệ")]
        public decimal Price { get; set; }

        [Range(1, 10000, ErrorMessage = "Thời gian không hợp lệ")]
        public int EstimatedDuration { get; set; }

        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
