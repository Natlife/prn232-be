using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTOs
{
    public class MaintenancePackageDTO
    {
        public int PackageId { get; set; }

        [Required(ErrorMessage = "T?n g?i kh?ng du?c tr?ng")]
        [StringLength(150, ErrorMessage = "T?n g?i kh?ng qu? 150 k? t?")]
        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Gi? kh?ng h?p l?")]
        public decimal Price { get; set; }

        [Range(1, 10000, ErrorMessage = "Th?i gian kh?ng h?p l?")]
        public int EstimatedDuration { get; set; }

        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
