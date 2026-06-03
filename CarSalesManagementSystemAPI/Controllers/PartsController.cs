using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Services;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;

namespace CarSalesManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {
            _partService = partService;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<Part>> Get()
        {
            try
            {
                var parts = _partService.GetAllParts();
                return Ok(parts.AsQueryable());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{key}")]
        [EnableQuery]
        public ActionResult<Part> Get(int key)
        {
            try
            {
                var part = _partService.GetPartById(key);
                if (part == null)
                {
                    return NotFound(new { message = "Không tìm thấy phụ tùng yêu cầu." });
                }
                return Ok(part);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] Part part)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                part.CreatedAt = DateTime.Now;
                _partService.AddPart(part);
                return CreatedAtAction(nameof(Get), new { key = part.PartId }, part);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromBody] Part part)
        {
            try
            {
                if (id != part.PartId)
                {
                    return BadRequest(new { message = "Mã ID phụ tùng không khớp." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _partService.UpdatePart(part);
                return Ok(new { success = true, message = "Cập nhật phụ tùng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var part = _partService.GetPartById(id);
                if (part == null)
                {
                    return NotFound(new { message = "Không tìm thấy phụ tùng cần xóa." });
                }
                _partService.DeletePart(id);
                return Ok(new { success = true, message = "Xóa phụ tùng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
