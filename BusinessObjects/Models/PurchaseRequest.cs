using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class PurchaseRequest
{
    public int RequestId { get; set; }

    public int CarId { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public string? Message { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual AppUser Customer { get; set; } = null!;
}
