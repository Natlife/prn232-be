using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BusinessObjects.Models;
using BusinessObjects.DTOs;
using Services;
using Microsoft.AspNetCore.Authorization;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComboOrdersController : ControllerBase
    {
        private readonly IComboOrderService _comboOrderService;

        public ComboOrdersController(IComboOrderService comboOrderService)
        {
            _comboOrderService = comboOrderService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ComboOrder>>> Get()
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized(new ApiResponse<object>(false, "Không xác định được danh tính người dùng."));
                }

                int userId = int.Parse(userIdStr);

                if (role == "Admin")
                {
                    var orders = _comboOrderService.GetAllOrders();
                    return Ok(new ApiResponse<IEnumerable<ComboOrder>>(true, "Success", orders));
                }
                else
                {
                    var orders = _comboOrderService.GetOrdersByCustomerId(userId);
                    return Ok(new ApiResponse<IEnumerable<ComboOrder>>(true, "Success", orders));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, "Lỗi hệ thống: " + ex.Message));
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<ComboOrder>> Get(int id)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized(new ApiResponse<object>(false, "Không xác định được danh tính."));
                }

                int userId = int.Parse(userIdStr);
                var order = _comboOrderService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound(new ApiResponse<object>(false, "Không tìm thấy đơn hàng combo."));
                }

                if (role != "Admin" && order.CustomerId != userId)
                {
                    return Forbid();
                }

                return Ok(new ApiResponse<ComboOrder>(true, "Success", order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, "Lỗi hệ thống: " + ex.Message));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ComboOrder order)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized(new ApiResponse<object>(false, "Vui lòng đăng nhập để đặt hàng."));
                }

                order.CustomerId = int.Parse(userIdStr);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(false, "Dữ liệu không hợp lệ.", ModelState));
                }

                _comboOrderService.AddOrder(order);
                return CreatedAtAction(nameof(Get), new { id = order.ComboOrderId }, new ApiResponse<ComboOrder>(true, "Đặt hàng thành công", order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, "Lỗi hệ thống: " + ex.Message));
            }
        }

        [HttpPost("{id}/generate-captcha")]
        [Authorize(Roles = "Admin")]
        public IActionResult GenerateCaptcha(int id, [FromBody] ComboOrderCaptchaGenerateDto dto)
        {
            try
            {
                _comboOrderService.GenerateCaptcha(id, dto.Code);
                var order = _comboOrderService.GetOrderById(id);
                string? generatedCode = order?.PurchaseType == "Deposit" ? order?.CaptchaCode : order?.FinalCaptchaCode;
                return Ok(new ApiResponse<object>(true, $"Sinh mã xác nhận thành công: {generatedCode}", new { captcha = generatedCode }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, "Lỗi hệ thống: " + ex.Message));
            }
        }

        [HttpPost("{id}/verify-captcha")]
        [Authorize(Roles = "Admin")]
        public IActionResult VerifyCaptcha(int id, [FromBody] ComboOrderCaptchaVerifyDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.CaptchaCode))
                {
                    return BadRequest(new ApiResponse<object>(false, "Vui lòng nhập mã Captcha."));
                }

                bool isVerified = _comboOrderService.VerifyCaptcha(id, dto.CaptchaCode);
                if (isVerified)
                {
                    return Ok(new ApiResponse<object>(true, "Xác nhận thanh toán và duyệt đơn hàng thành công!"));
                }
                else
                {
                    return BadRequest(new ApiResponse<object>(false, "Mã xác thực không đúng. Vui lòng kiểm tra lại."));
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, "Lỗi hệ thống: " + ex.Message));
            }
        }

        [HttpPost("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized(new ApiResponse<object>(false, "Không xác định được danh tính."));
                }

                int userId = int.Parse(userIdStr);
                var order = _comboOrderService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound(new ApiResponse<object>(false, "Không tìm thấy đơn hàng."));
                }

                if (role != "Admin" && order.CustomerId != userId)
                {
                    return Forbid();
                }

                _comboOrderService.CancelOrder(id);
                return Ok(new ApiResponse<object>(true, "Đã hủy đơn hàng thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, "Lỗi hệ thống: " + ex.Message));
            }
        }
    }
}
