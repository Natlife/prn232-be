using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace CarSalesManagementSystemAPI.Controllers;

// ─── FLOW ─────────────────────────────────────────────────────────────────────
//
//  POST /api/combo-orders/draft-preview  (anonymous)
//    → Decode draft token → resolve items server-side → return preview
//
//  POST /api/combo-orders  [Auth: any logged-in]
//    → Extract customerId + name from JWT → PlaceOrder via service
//
//  GET  /api/combo-orders           [Admin: all | Customer: own]
//  GET  /api/combo-orders/{id}      [Admin: any | Customer: own only]
//  PATCH /api/combo-orders/{id}/status  [Admin only]
//
// ─────────────────────────────────────────────────────────────────────────────

[Route("api/combo-orders")]
[ApiController]
public class ComboOrdersController : ControllerBase
{
    private static readonly HashSet<string> ValidStatuses =
        new() { "Pending", "Confirmed", "Processing", "Completed", "Cancelled" };

    private readonly IComboOrderService _comboOrderService;
    private readonly ILogger<ComboOrdersController> _logger;

    public ComboOrdersController(
        IComboOrderService comboOrderService,
        ILogger<ComboOrdersController> logger)
    {
        _comboOrderService = comboOrderService;
        _logger = logger;
    }

    /// <summary>
    /// Decodes a Base64 draft token and returns a full price preview.
    /// Used by Confirm page before user submits the order.
    /// </summary>
    [HttpPost("draft-preview")]
    [AllowAnonymous]
    public IActionResult DraftPreview([FromBody] string draftToken)
    {
        if (string.IsNullOrWhiteSpace(draftToken))
            return BadRequest(new { message = "Draft token không được trống." });

        List<ComboOrderItemInputDto> items;
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(draftToken));
            items = JsonSerializer.Deserialize<List<ComboOrderItemInputDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<ComboOrderItemInputDto>();
        }
        catch
        {
            return BadRequest(new { message = "Draft token không hợp lệ hoặc đã hết hạn." });
        }

        try
        {
            var preview = _comboOrderService.PreviewDraft(items);
            preview.DraftToken = draftToken;
            return Ok(new ApiResponse<ComboOrderPreviewDto>(true, "OK", preview));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Places a combo order. Requires authentication.
    /// Price is always re-resolved server-side from the draft token items.
    /// </summary>
    [HttpPost]
    [Authorize]
    public IActionResult PlaceOrder([FromBody] ComboOrderCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (customerId, customerName, extractError) = ExtractCallerIdentity();
        if (extractError is not null)
            return Unauthorized(new { message = extractError });

        var source = string.IsNullOrEmpty(dto.ChatSessionId) ? "manual" : "chatbot";

        try
        {
            var order = _comboOrderService.PlaceOrder(dto, customerId, customerName!, source);
            _logger.LogInformation(
                "ComboOrder placed. OrderId={OrderId} CustomerId={CustomerId} Source={Source}",
                order.ComboOrderId, customerId, source);

            return CreatedAtAction(nameof(GetById), new { id = order.ComboOrderId },
                new ApiResponse<object>(true, "Đặt hàng thành công. Nhân viên sẽ liên hệ xác nhận sớm.",
                    new { order.ComboOrderId, order.TotalAmount, order.Status }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to place ComboOrder for customer {CustomerId}", customerId);
            return StatusCode(500, new { message = "Lỗi hệ thống. Vui lòng thử lại." });
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var (customerId, _, extractError) = ExtractCallerIdentity();
        if (extractError is not null) return Unauthorized(new { message = extractError });

        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

        var orders = isAdmin
            ? _comboOrderService.GetAllOrders()
            : _comboOrderService.GetOrdersByCustomerId(customerId);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        var (customerId, _, extractError) = ExtractCallerIdentity();
        if (extractError is not null) return Unauthorized(new { message = extractError });

        var order = _comboOrderService.GetOrderById(id);
        if (order is null)
            return NotFound(new { message = $"Không tìm thấy đơn hàng #{id}." });

        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        if (!isAdmin && order.CustomerId != customerId)
            return Forbid();

        return Ok(order);
    }

    /// <summary>Admin-only: transitions order to a new status.</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateStatus(int id, [FromBody] ComboOrderStatusPatchDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!ValidStatuses.Contains(dto.Status))
            return BadRequest(new { message = $"Trạng thái '{dto.Status}' không hợp lệ." });

        try
        {
            _comboOrderService.UpdateStatus(id, dto.Status);
            return Ok(new { success = true, message = $"Đã cập nhật trạng thái thành '{dto.Status}'." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for ComboOrder {OrderId}", id);
            return StatusCode(500, new { message = "Lỗi hệ thống." });
        }
    }

    // ─── PRIVATE ─────────────────────────────────────────────────────────────

    private (int customerId, string? customerName, string? error) ExtractCallerIdentity()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return (0, null, "Không xác định được danh tính người dùng.");

        var name = User.FindFirst(ClaimTypes.Name)?.Value
                   ?? User.FindFirst("name")?.Value
                   ?? "Khách hàng";

        return (userId, name, null);
    }
}
