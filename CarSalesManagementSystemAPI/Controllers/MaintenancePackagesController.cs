using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Services;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Formatter;

namespace CarSalesManagementSystemAPI.Controllers
{
    public class MaintenancePackagesController : ODataController
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

        [HttpGet]
        [EnableQuery]
        public ActionResult<MaintenancePackage> Get([FromODataUri] int key)
        {
            var package = _service.GetPackageById(key);
            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }

        [HttpGet("/odata/MaintenancePackages/available")]
        public ActionResult<IEnumerable<MaintenancePackage>> GetAvailable()
        {
            return Ok(_service.GetAvailablePackages());
        }

        [HttpPost]
        public IActionResult Post([FromBody] MaintenancePackage package)
        {
            _service.AddPackage(package);
            return Created(package);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri] int key, [FromBody] MaintenancePackage package)
        {
            if (key != package.PackageId)
            {
                return BadRequest();
            }
            _service.UpdatePackage(package);
            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int key)
        {
            _service.DeletePackage(key);
            return NoContent();
        }
    }
}
