using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTOs;

// ─── CHAT ────────────────────────────────────────────────────────────────────

public class ChatRequestDto
{
    [Required(ErrorMessage = "SessionId không được để trống")]
    public string SessionId { get; set; } = null!;

    [Required(ErrorMessage = "Tin nhắn không được để trống")]
    [StringLength(2000, ErrorMessage = "Tin nhắn không vượt quá 2000 ký tự")]
    public string Message { get; set; } = null!;

    public int? CustomerId { get; set; }
}

public class SuggestedItemDto
{
    public string ItemType { get; set; } = null!;
    public int ItemId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string DetailUrl { get; set; } = null!;
}

public class ChatResponseDto
{
    public string Reply { get; set; } = null!;
    public List<SuggestedItemDto> SuggestedItems { get; set; } = new();
    public string? OrderLink { get; set; }
    public ChatActionDto? Action { get; set; }
    public bool HasOrderSuggestion { get; set; }
    public string SessionId { get; set; } = null!;
}

public class ChatActionDto
{
    public string Type { get; set; } = null!;
    public string TargetType { get; set; } = null!;
    public int TargetId { get; set; }
    public string Label { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool AutoOpenPopup { get; set; }
}

// ─── COMBO ORDER ─────────────────────────────────────────────────────────────

public class ComboOrderItemInputDto
{
    [Required(ErrorMessage = "ItemType bắt buộc")]
    public string ItemType { get; set; } = null!;   // Car | Part | Service

    [Range(1, int.MaxValue, ErrorMessage = "ReferenceId phải lớn hơn 0")]
    public int ReferenceId { get; set; }

    [Range(1, 100, ErrorMessage = "Số lượng từ 1 đến 100")]
    public int Quantity { get; set; } = 1;
}

public class ComboOrderCreateDto
{
    [Required(ErrorMessage = "Số điện thoại không được trống")]
    [StringLength(20)]
    public string CustomerPhone { get; set; } = null!;

    [Required(ErrorMessage = "Loại giao dịch là bắt buộc")]
    [RegularExpression("Deposit|Buyout", ErrorMessage = "Loại giao dịch chỉ nhận Deposit hoặc Buyout")]
    public string PurchaseType { get; set; } = "Buyout";

    [StringLength(255)]
    public string? ShippingAddress { get; set; }

    [StringLength(1000)]
    public string? Note { get; set; }

    public string? ChatSessionId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
    public List<ComboOrderItemInputDto> Items { get; set; } = new();
}

public class ComboOrderItemPreviewDto
{
    public string ItemType { get; set; } = null!;
    public int ReferenceId { get; set; }
    public string Name { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
    public string? ImageUrl { get; set; }
}

public class ComboOrderPreviewDto
{
    public List<ComboOrderItemPreviewDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string DraftToken { get; set; } = null!;
}

public class ComboOrderStatusPatchDto
{
    [Required]
    public string Status { get; set; } = null!;
}

public class ComboOrderCaptchaGenerateDto
{
    [StringLength(20)]
    public string? Code { get; set; }
}

public class ComboOrderCaptchaVerifyDto
{
    [Required(ErrorMessage = "Mã captcha không được để trống")]
    [StringLength(20)]
    public string CaptchaCode { get; set; } = null!;
}
