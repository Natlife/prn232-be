using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class MaintenanceAppointment
{
    public int AppointmentId { get; set; }

    public int CustomerId { get; set; }

    public int PackageId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public string CarName { get; set; } = null!;

    public string? LicensePlate { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string? Note { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AppUser Customer { get; set; } = null!;

    public virtual MaintenancePackage Package { get; set; } = null!;
}
