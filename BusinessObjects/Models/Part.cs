using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Part
{
    public int PartId { get; set; }

    public int CategoryId { get; set; }

    public string PartName { get; set; } = null!;

    public string PartCode { get; set; } = null!;

    public string? Brand { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual PartCategory Category { get; set; } = null!;

    public virtual ICollection<PartOrderDetail> PartOrderDetails { get; set; } = new List<PartOrderDetail>();

    public virtual ICollection<ServiceRequiredPart> ServiceRequiredParts { get; set; } = new List<ServiceRequiredPart>();

    public virtual ICollection<AppointmentConsumedPart> AppointmentConsumedParts { get; set; } = new List<AppointmentConsumedPart>();
}
