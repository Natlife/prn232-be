using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using BusinessObjects.Models;
using Services;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarBrandsController : ControllerBase
    {
        private readonly ICarBrandService _brandService;

        public CarBrandsController(ICarBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CarBrand>> Get()
        {
            try
            {
                var brands = _brandService.GetAllBrands();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CarBrand> GetById(int id)
        {
            try
            {
                var brand = _brandService.GetBrandById(id);
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
