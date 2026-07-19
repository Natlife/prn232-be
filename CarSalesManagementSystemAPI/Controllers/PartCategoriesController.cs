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
    [ApiController]
    public class PartCategoriesController : ControllerBase
    {
        private readonly IPartCategoryService _service;

        public PartCategoriesController(IPartCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<PartCategory>> Get()
        {
            try
            {
                var categories = _service.GetAllCategories();
                return Ok(categories.AsQueryable());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<PartCategory> GetById(int id)
        {
            try
            {
                var category = _service.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục yêu cầu." });
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] PartCategory category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _service.AddCategory(category);
                return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
