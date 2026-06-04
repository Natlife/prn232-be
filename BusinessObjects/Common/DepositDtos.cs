using System;
using BusinessObjects.Models;
using BusinessObjects.Common;

namespace BusinessObjects.Common;

public class DepositRequest
{
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? CustomerEmail { get; set; }
    public string CaptchaCode { get; set; } = null!;
}

public class DepositResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? RequestId { get; set; }
    public decimal? DepositAmount { get; set; }
    public DateTime? DepositExpiry { get; set; }
}
