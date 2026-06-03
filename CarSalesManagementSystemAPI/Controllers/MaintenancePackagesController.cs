using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Services;
using Microsoft.AspNetCore.OData.Query;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [Route("odata/[controller]")]
    [ApiController]
    public class MaintenancePackagesController : ControllerBase
    {
        private readonly IMaintenancePackageService _service;

        public MaintenancePackagesController(IMaintenancePackageService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<MaintenancePackage>> Get()
        {
            return Ok(_service.GetAllPackages().AsQueryable());
        }

        [HttpGet("available")]
        public ActionResult<IEnumerable<MaintenancePackage>> GetAvailable()
        {
            return Ok(_service.GetAvailablePackages());
        }

        [HttpGet("{key}")]
        [EnableQuery]
        public ActionResult<MaintenancePackage> Get(int key)
        {
            var package = _service.GetPackageById(key);
            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }

        [HttpPost]
        public IActionResult Post([FromBody] MaintenancePackage package)
        {
            _service.AddPackage(package);
            return CreatedAtAction(nameof(Get), new { id = package.PackageId }, package);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MaintenancePackage package)
        {
            if (id != package.PackageId)
            {
                return BadRequest();
            }
            _service.UpdatePackage(package);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.DeletePackage(id);
            return NoContent();
        }
    }
}
