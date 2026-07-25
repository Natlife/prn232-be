using System;
using System.Collections.Generic;

namespace BusinessObjects.DTOs
{
    public class ChatRequestDto
    {
        public string? SessionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string? FrontendOrigin { get; set; }   // origin thật của trình duyệt (scheme://host)
    }

    public class ChatResponseDto
    {
        public string Reply { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public List<ChatSuggestedItemDto> SuggestedItems { get; set; } = new List<ChatSuggestedItemDto>();
        public string? OrderLink { get; set; }
        public ChatActionDto? Action { get; set; }
        public bool HasOrderSuggestion { get; set; }
    }

    // Khớp CHÍNH XÁC với Python ChatResponse.suggested_items
    public class ChatSuggestedItemDto
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string DetailUrl { get; set; } = string.Empty;
    }

    // Khớp CHÍNH XÁC với Python ChatResponse.action (object, KHÔNG phải string)
    public class ChatActionDto
    {
        public string Type { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public int TargetId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool AutoOpenPopup { get; set; }
    }
}
