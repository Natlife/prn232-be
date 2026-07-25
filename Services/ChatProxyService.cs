using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services;

// ─── FLOW ─────────────────────────────────────────────────────────────────────
//
//  SendMessageAsync(request)
//    → POST {PythonService:BaseUrl}/chat
//    → Deserialize ChatResponseDto from Python response
//    → On any failure → return graceful fallback reply (never throw to caller)
//
//  GetHistoryAsync(sessionId)
//    → GET {PythonService:BaseUrl}/chat/history/{sessionId}
//    → Return raw JSON object to controller
//
//  Boundary: this service ONLY handles HTTP transport to Python.
//            Toàn bộ nghiệp vụ tư vấn nằm ở dịch vụ Python RAG (chatbot chỉ tư vấn, không tạo đơn).
//
// ─────────────────────────────────────────────────────────────────────────────

public class ChatProxyService : IChatProxyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatProxyService> _logger;
    private readonly string _pythonBaseUrl;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    public ChatProxyService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ChatProxyService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _pythonBaseUrl = configuration["PythonService:BaseUrl"]
            ?? throw new InvalidOperationException("PythonService:BaseUrl is not configured.");
    }

    public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_pythonBaseUrl}/chat",
                request,
                _jsonOptions);

            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Python /chat returned {Status}: {Body}", (int)response.StatusCode, raw);
                return BuildFallbackReply(request.SessionId,
                    "Dịch vụ tư vấn AI đang bận. Vui lòng thử lại sau giây lát.");
            }

            // 1) Thử parse đầy đủ theo DTO
            try
            {
                var result = JsonSerializer.Deserialize<ChatResponseDto>(raw, _jsonOptions);
                if (result != null && !string.IsNullOrWhiteSpace(result.Reply))
                    return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize Python chat response. Raw={Raw}", raw);
            }

            // 2) Cứu vãn: ít nhất lấy 'reply' để khách vẫn thấy nội dung tư vấn
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("reply", out var replyEl))
                    return BuildFallbackReply(request.SessionId,
                        replyEl.GetString() ?? "Xin lỗi, tôi chưa lấy được nội dung phản hồi.");
            }
            catch { /* raw không phải JSON hợp lệ */ }

            return BuildFallbackReply(request.SessionId, "Không nhận được phản hồi hợp lệ từ dịch vụ AI.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Python chat service unreachable for session {SessionId}", request.SessionId);
            return BuildFallbackReply(request.SessionId,
                "Dịch vụ tư vấn AI tạm thời không khả dụng. Vui lòng thử lại sau hoặc liên hệ nhân viên showroom.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error proxying chat for session {SessionId}", request.SessionId);
            return BuildFallbackReply(request.SessionId,
                "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
        }
    }

    public async Task<object> GetHistoryAsync(string sessionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_pythonBaseUrl}/chat/history/{sessionId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<object>();
            return json ?? new { session_id = sessionId, messages = Array.Empty<object>(), count = 0 };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch chat history for session {SessionId}", sessionId);
            return new { session_id = sessionId, messages = Array.Empty<object>(), count = 0 };
        }
    }

    private static ChatResponseDto BuildFallbackReply(string sessionId, string message) =>
        new()
        {
            Reply = message,
            SessionId = sessionId,
            SuggestedItems = new(),
            Action = null,
            HasOrderSuggestion = false,
        };
}
