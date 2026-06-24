using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class ComboOrder
{
    public int ComboOrderId { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public string? ShippingAddress { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Note { get; set; }

    public string Source { get; set; } = "manual";

    public string? ChatSessionId { get; set; }

    public string PurchaseType { get; set; } = "Buyout";

    public string Status { get; set; } = "Pending";

    public decimal? DepositAmount { get; set; }

    public DateTime? DepositExpiresAt { get; set; }

    public string? CaptchaCode { get; set; }

    public DateTime? CaptchaGeneratedAt { get; set; }

    public bool IsCaptchaUsed { get; set; }

    public DateTime? CaptchaUsedAt { get; set; }

    public string? FinalCaptchaCode { get; set; }

    public DateTime? FinalCaptchaGeneratedAt { get; set; }

    public bool IsFinalCaptchaUsed { get; set; }

    public DateTime? FinalCaptchaUsedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AppUser Customer { get; set; } = null!;

    public virtual ICollection<ComboOrderItem> Items { get; set; } = new List<ComboOrderItem>();
}
