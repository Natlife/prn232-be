using Microsoft.AspNetCore.Mvc;
using Services;
using BusinessObjects.Common;

namespace CarSalesManagementSystemAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IPurchaseRequestService _service;

    public PurchaseRequestsController(IPurchaseRequestService service)
    {
        _service = service;
    }

    [HttpPost("deposit")]
    public IActionResult CreateDeposit([FromBody] DepositRequest request)
    {
        if (request == null)
            return BadRequest("Yêu cầu không hợp lệ.");

        var result = _service.CreateDeposit(request);
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
}
