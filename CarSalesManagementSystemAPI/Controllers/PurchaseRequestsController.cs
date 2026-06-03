using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using BusinessObjects.Common;

namespace CarSalesManagementSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("odata/[controller]")]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IPurchaseRequestService _service;

    public PurchaseRequestsController(IPurchaseRequestService service)
    {
        _service = service;
    }

    [HttpPost("deposit")]
    [Authorize]
    public IActionResult CreateDeposit([FromBody] DepositRequest request)
    {
        if (request == null)
            return BadRequest("Yêu cầu không hợp lệ.");

        var result = _service.CreateDeposit(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("buyout")]
    [Authorize]
    public IActionResult CreateBuyout([FromBody] DepositRequest request)
    {
        if (request == null)
            return BadRequest("Yêu cầu không hợp lệ.");

        var result = _service.CreateBuyout(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("customer/{customerId}")]
    public IActionResult GetDepositsByCustomer(int customerId)
    {
        var deposits = _service.GetDepositsByCustomer(customerId);
        return Ok(deposits);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Microsoft.AspNetCore.OData.Query.EnableQuery]
    public ActionResult<IQueryable<BusinessObjects.Models.PurchaseRequest>> Get()
    {
        try
        {
            var result = _service.GetAllPurchaseRequests();
            return Ok(result.AsQueryable());
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}
