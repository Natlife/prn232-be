using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace CarSalesManagementSystemAPI.Controllers;

[Route("api/combo-orders")]
[ApiController]
public class ComboOrdersController : ControllerBase
{
    private static readonly HashSet<string> ValidStatuses =
        new() { "Pending", "Deposited", "Completed", "Cancelled", "DepositExpired" };

    private readonly IComboOrderService _comboOrderService;
    private readonly ILogger<ComboOrdersController> _logger;

    public ComboOrdersController(IComboOrderService comboOrderService, ILogger<ComboOrdersController> logger)
    {
        _comboOrderService = comboOrderService;
        _logger = logger;
    }

    [HttpPost("draft-preview")]
    [AllowAnonymous]
    public IActionResult DraftPreview([FromBody] string draftToken)
    {
        if (string.IsNullOrWhiteSpace(draftToken))
        {
            return BadRequest(new { message = "Draft token khong duoc de trong." });
        }

        List<ComboOrderItemInputDto> items;
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(draftToken));
            items = JsonSerializer.Deserialize<List<ComboOrderItemInputDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ComboOrderItemInputDto>();
        }
        catch
        {
            return BadRequest(new { message = "Draft token khong hop le hoac da het han." });
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

    [HttpPost]
    [Authorize]
    public IActionResult PlaceOrder([FromBody] ComboOrderCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (customerId, customerName, extractError) = ExtractCallerIdentity();
        if (extractError is not null)
        {
            return Unauthorized(new { message = extractError });
        }

        var source = string.IsNullOrEmpty(dto.ChatSessionId) ? "manual" : "chatbot";

        try
        {
            var order = _comboOrderService.PlaceOrder(dto, customerId, customerName!, source);
            return CreatedAtAction(nameof(GetById), new { id = order.ComboOrderId },
                new ApiResponse<object>(true, "Don hang combo da duoc tao.",
                    new
                    {
                        order.ComboOrderId,
                        order.TotalAmount,
                        order.Status,
                        order.PurchaseType,
                        order.DepositAmount
                    }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to place combo order for customer {CustomerId}", customerId);
            return StatusCode(500, new { message = "Loi he thong. Vui long thu lai." });
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var (customerId, _, extractError) = ExtractCallerIdentity();
        if (extractError is not null)
        {
            return Unauthorized(new { message = extractError });
        }

        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        var orders = isAdmin
            ? _comboOrderService.GetAllOrders()
            : _comboOrderService.GetOrdersByCustomerId(customerId);

        if (!isAdmin)
        {
            foreach (var order in orders)
            {
                RedactSensitiveFieldsForCustomer(order);
            }
        }

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        var (customerId, _, extractError) = ExtractCallerIdentity();
        if (extractError is not null)
        {
            return Unauthorized(new { message = extractError });
        }

        var order = _comboOrderService.GetOrderById(id);
        if (order is null)
        {
            return NotFound(new { message = $"Khong tim thay don hang #{id}." });
        }

        var isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
        if (!isAdmin && order.CustomerId != customerId)
        {
            return Forbid();
        }

        if (!isAdmin)
        {
            RedactSensitiveFieldsForCustomer(order);
        }

        return Ok(order);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateStatus(int id, [FromBody] ComboOrderStatusPatchDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!ValidStatuses.Contains(dto.Status))
        {
            return BadRequest(new { message = $"Trang thai '{dto.Status}' khong hop le." });
        }

        try
        {
            _comboOrderService.UpdateStatus(id, dto.Status);
            return Ok(new { success = true, message = $"Da cap nhat trang thai thanh '{dto.Status}'." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for combo order {OrderId}", id);
            return StatusCode(500, new { message = "Loi he thong." });
        }
    }

    [HttpPost("{id:int}/generate-captcha")]
    [Authorize(Roles = "Admin")]
    public IActionResult GenerateCaptcha(int id, [FromBody] ComboOrderCaptchaGenerateDto? dto)
    {
        try
        {
            var order = _comboOrderService.GenerateCaptcha(id, dto?.Code);
            var captchaInfo = BuildActiveCaptchaInfo(order);

            return Ok(new
            {
                success = true,
                message = captchaInfo.Stage == "deposit"
                    ? "Da tao captcha dat coc cho don combo."
                    : string.Equals(order.PurchaseType, "Deposit", StringComparison.OrdinalIgnoreCase)
                        ? "Da tao captcha mua dut lan hai cho don combo."
                        : "Da tao captcha mua dut cho don combo.",
                data = new
                {
                    order.ComboOrderId,
                    order.PurchaseType,
                    order.Status,
                    captchaInfo.Stage,
                    captchaInfo.CaptchaCode,
                    captchaInfo.GeneratedAt,
                    captchaInfo.IsUsed
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate combo captcha for order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Loi he thong." });
        }
    }

    [HttpPost("{id:int}/verify-captcha")]
    [Authorize]
    public IActionResult VerifyCaptcha(int id, [FromBody] ComboOrderCaptchaVerifyDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (customerId, _, extractError) = ExtractCallerIdentity();
        if (extractError is not null)
        {
            return Unauthorized(new { success = false, message = extractError });
        }

        try
        {
            var order = _comboOrderService.VerifyCaptcha(id, customerId, dto.CaptchaCode);
            var successMessage = order.Status == "Deposited"
                ? "Dat coc combo thanh cong. Don se duoc giu cho trong 7 ngay, sau do ban co the dung captcha lan hai de mua dut."
                : "Mua dut combo thanh cong. Don hang da duoc hoan tat.";

            return Ok(new
            {
                success = true,
                message = successMessage,
                data = new
                {
                    order.ComboOrderId,
                    order.Status,
                    order.PurchaseType,
                    order.DepositAmount,
                    order.TotalAmount,
                    order.DepositExpiresAt,
                    order.CaptchaUsedAt,
                    order.FinalCaptchaUsedAt
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify combo captcha for order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Loi he thong." });
        }
    }

    private (int customerId, string? customerName, string? error) ExtractCallerIdentity()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return (0, null, "Khong xac dinh duoc danh tinh nguoi dung.");
        }

        var name = User.FindFirst(ClaimTypes.Name)?.Value
                   ?? User.FindFirst("name")?.Value
                   ?? "Khach hang";

        return (userId, name, null);
    }

    private static ComboOrderCaptchaInfoDto BuildActiveCaptchaInfo(BusinessObjects.Models.ComboOrder order)
    {
        if (string.Equals(order.PurchaseType, "Deposit", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(order.Status, "Deposited", StringComparison.OrdinalIgnoreCase))
        {
            return new ComboOrderCaptchaInfoDto
            {
                Stage = "buyout",
                CaptchaCode = order.FinalCaptchaCode,
                GeneratedAt = order.FinalCaptchaGeneratedAt,
                IsUsed = order.IsFinalCaptchaUsed
            };
        }

        if (string.Equals(order.PurchaseType, "Buyout", StringComparison.OrdinalIgnoreCase))
        {
            return new ComboOrderCaptchaInfoDto
            {
                Stage = "buyout",
                CaptchaCode = order.CaptchaCode,
                GeneratedAt = order.CaptchaGeneratedAt,
                IsUsed = order.IsCaptchaUsed
            };
        }

        return new ComboOrderCaptchaInfoDto
        {
            Stage = "deposit",
            CaptchaCode = order.CaptchaCode,
            GeneratedAt = order.CaptchaGeneratedAt,
            IsUsed = order.IsCaptchaUsed
        };
    }

    private static void RedactSensitiveFieldsForCustomer(BusinessObjects.Models.ComboOrder order)
    {
        order.CaptchaCode = null;
        order.FinalCaptchaCode = null;
    }
}
