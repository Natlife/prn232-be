using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class MaintenancePackage
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int EstimatedDuration { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MaintenanceAppointment> MaintenanceAppointments { get; set; } = new List<MaintenanceAppointment>();
}
