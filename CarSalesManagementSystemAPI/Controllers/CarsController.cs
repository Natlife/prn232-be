using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using BusinessObjects.Common;
using Services;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Formatter;

namespace CarSalesManagementSystemAPI.Controllers
{
    public class CarsController : ODataController
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

        [HttpGet]
        [EnableQuery]
        public ActionResult<Car> Get([FromODataUri] int key)
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                return Created(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Put([FromODataUri] int key, [FromBody] Car car)
        {
            try
            {
                if (key != car.CarId)
                {
                    return BadRequest(new { message = "Mã xe không trùng khớp." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _carService.UpdateCar(car);
                return Ok(new { success = true, message = "Cập nhật xe thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromODataUri] int key)
        {
            try
            {
                var car = _carService.GetCarById(key);
                if (car == null)
                {
                    return NotFound(new { message = "Không tìm thấy xe cần xóa." });
                }
                _carService.DeleteCar(key);
                return Ok(new { success = true, message = "Xóa xe thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
