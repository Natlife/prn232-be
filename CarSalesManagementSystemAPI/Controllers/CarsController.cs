using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using BusinessObjects.Common;
using Services;
using Microsoft.AspNetCore.OData.Query;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<Car>> Get()
        {
            try
            {
                var cars = _carService.GetAllCars();
                return Ok(cars.AsQueryable());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{key}")]
        [EnableQuery]
        public ActionResult<Car> Get(int key)
        {
            try
            {
                var car = _carService.GetCarById(key);
                if (car == null)
                {
                    return NotFound(new { message = "Không tìm thấy xe yêu cầu." });
                }
                return Ok(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("paged")]
        public ActionResult<PagedResult<Car>> GetPaged([FromQuery] CarSearchRequest request)
        {
            try
            {
                var result = _carService.GetPagedCars(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<Car>> GetAll()
        {
            try
            {
                var result = _carService.GetAllCars();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Car car)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                car.CreatedAt = DateTime.Now;
                _carService.AddCar(car);
                return CreatedAtAction(nameof(Get), new { key = car.CarId }, car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Car car)
        {
            try
            {
                if (id != car.CarId)
                {
                    return BadRequest(new { message = "Mã xe không trùng khớp." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existing = _carService.GetCarById(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Không tìm thấy xe cần cập nhật." });
                }
                
                car.CreatedAt = existing.CreatedAt;

                _carService.UpdateCar(car);
                return Ok(new { success = true, message = "Cập nhật xe thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var car = _carService.GetCarById(id);
                if (car == null)
                {
                    return NotFound(new { message = "Không tìm thấy xe cần xóa." });
                }
                _carService.DeleteCar(id);
                return Ok(new { success = true, message = "Xóa xe thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
