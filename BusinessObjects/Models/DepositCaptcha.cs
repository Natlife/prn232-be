using System;

namespace BusinessObjects.Models;

public partial class DepositCaptcha
{
    public int CaptchaId { get; set; }
    public string Code { get; set; } = null!;
    public int CarId { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public virtual Car Car { get; set; } = null!;
}
