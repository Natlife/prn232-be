using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class CustomerCar
    {
        public int CustomerCarId { get; set; }
        public int CustomerId { get; set; }
        public int BrandId { get; set; }
        public string Model { get; set; } = null!;
        public int? Year { get; set; }
        public string? VIN { get; set; }
        public string LicensePlate { get; set; } = null!;
        public string? Color { get; set; }
        public DateTime? ExpiredAt { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CreatedUser { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedUser { get; set; }

        public virtual AppUser Customer { get; set; } = null!;
        public virtual CarBrand Brand { get; set; } = null!;
        public virtual AppUser? CreatedUserNavigation { get; set; }
        public virtual AppUser? UpdatedUserNavigation { get; set; }
    }
}
