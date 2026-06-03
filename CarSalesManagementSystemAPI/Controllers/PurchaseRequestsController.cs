using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using BusinessObjects.Common;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;
using System.Security.Claims;

namespace CarSalesManagementSystemAPI.Controllers
{
    public class PurchaseRequestsController : ODataController
    {
        private readonly IPurchaseRequestService _service;

        public PurchaseRequestsController(IPurchaseRequestService service)
        {
            _service = service;
        }

        [HttpPost("/odata/PurchaseRequests/deposit")]
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

        [HttpPost("/odata/PurchaseRequests/buyout")]
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

        [HttpGet]
        [Authorize]
        [EnableQuery]
        public ActionResult<IQueryable<BusinessObjects.Models.PurchaseRequest>> Get()
        {
            try
            {
                var query = _service.GetAllPurchaseRequests().AsQueryable();

                if (!User.IsInRole("Admin"))
                {
                    var customerIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

                    if (string.IsNullOrWhiteSpace(customerIdValue) || !int.TryParse(customerIdValue, out var customerId))
                    {
                        return Forbid();
                    }

                    query = query.Where(r => r.CustomerId == customerId);
                }

                return Ok(query);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
