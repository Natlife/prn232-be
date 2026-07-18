using System;
using System.Collections.Generic;

namespace BusinessObjects.DTOs
{
    public class ComboOrderItemDto
    {
        public int ReferenceId { get; set; }
        public string ItemType { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class ComboOrderCaptchaGenerateDto
    {
        public string? Code { get; set; }
    }

    public class ComboOrderCaptchaVerifyDto
    {
        public string CaptchaCode { get; set; } = null!;
    }
}
