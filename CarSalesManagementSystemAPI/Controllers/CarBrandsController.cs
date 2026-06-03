using Microsoft.AspNetCore.Mvc;
using System;
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
    public class CarBrandsController : ControllerBase
    {
        private readonly ICarBrandService _brandService;

        public CarBrandsController(ICarBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<CarBrand>> Get()
        {
            try
            {
                var brands = _brandService.GetAllBrands();
                return Ok(brands.AsQueryable());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{key}")]
        [EnableQuery]
        public ActionResult<CarBrand> Get(int key)
        {
            try
            {
                var brand = _brandService.GetBrandById(key);
                if (brand == null)
                {
                    return NotFound(new { message = "Không tìm thấy hãng xe." });
                }
                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
