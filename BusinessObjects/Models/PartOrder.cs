using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class PartOrder
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AppUser Customer { get; set; } = null!;

    public virtual ICollection<PartOrderDetail> PartOrderDetails { get; set; } = new List<PartOrderDetail>();
}
