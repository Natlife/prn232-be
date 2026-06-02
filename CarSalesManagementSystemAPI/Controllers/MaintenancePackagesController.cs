using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BusinessObjects.Models;
using Services;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenancePackagesController : ControllerBase
    {
        private readonly IMaintenancePackageService _service;

        public MaintenancePackagesController(IMaintenancePackageService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MaintenancePackage>> Get()
        {
            return Ok(_service.GetAllPackages());
        }

        [HttpGet("available")]
        public ActionResult<IEnumerable<MaintenancePackage>> GetAvailable()
        {
            return Ok(_service.GetAvailablePackages());
        }

        [HttpGet("{id}")]
        public ActionResult<MaintenancePackage> Get(int id)
        {
            var package = _service.GetPackageById(id);
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
