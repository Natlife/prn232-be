using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTOs;

public class ChatRequestDto
{
    [Required(ErrorMessage = "SessionId khong duoc de trong")]
    public string SessionId { get; set; } = null!;

    [Required(ErrorMessage = "Tin nhan khong duoc de trong")]
    [StringLength(2000, ErrorMessage = "Tin nhan khong vuot qua 2000 ky tu")]
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

public class ComboOrderItemInputDto
{
    [Required(ErrorMessage = "ItemType bat buoc")]
    public string ItemType { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "ReferenceId phai lon hon 0")]
    public int ReferenceId { get; set; }

    [Range(1, 100, ErrorMessage = "So luong tu 1 den 100")]
    public int Quantity { get; set; } = 1;
}

public class ComboOrderCreateDto
{
    [Required(ErrorMessage = "So dien thoai khong duoc de trong")]
    [StringLength(20)]
    public string CustomerPhone { get; set; } = null!;

    [Required(ErrorMessage = "Loai giao dich la bat buoc")]
    [RegularExpression("Deposit|Buyout", ErrorMessage = "Loai giao dich chi nhan Deposit hoac Buyout")]
    public string PurchaseType { get; set; } = "Buyout";

    [StringLength(255)]
    public string? ShippingAddress { get; set; }

    [StringLength(1000)]
    public string? Note { get; set; }

    public string? ChatSessionId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Don hang phai co it nhat 1 san pham")]
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
    [Required(ErrorMessage = "Ma captcha khong duoc de trong")]
    [StringLength(20)]
    public string CaptchaCode { get; set; } = null!;
}

public class ComboOrderCaptchaInfoDto
{
    public string Stage { get; set; } = null!;
    public string? CaptchaCode { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public bool IsUsed { get; set; }
}
