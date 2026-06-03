using Microsoft.AspNetCore.Mvc;
using Services;
using BusinessObjects.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.OData.Query;

namespace CarSalesManagementSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositCaptchasController : ControllerBase
{
    private readonly IDepositCaptchaService _service;

    public DepositCaptchasController(IDepositCaptchaService service)
    {
        _service = service;
    }

    [HttpGet]
    [EnableQuery]
    public ActionResult<IQueryable<DepositCaptcha>> Get()
    {
        try
        {
            var result = _service.GetAllCaptchas();
            return Ok(result.AsQueryable());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPost("generate")]
    public IActionResult Generate([FromBody] GenerateCaptchaRequest request)
    {
        if (request == null || request.CarId <= 0)
        {
            return BadRequest(new { success = false, message = "Thông tin xe không hợp lệ." });
        }

        try
        {
            var captcha = _service.GenerateCaptcha(request.CarId, request.Code);
            return Ok(new { success = true, message = "Tạo mã xác thực thành công.", data = captcha });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}

public class GenerateCaptchaRequest
{
    public int CarId { get; set; }
    public string? Code { get; set; }
}
