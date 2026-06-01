using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;

namespace ProductManagementMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductService repository = new ProductService();

        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts() => repository.GetProducts();

        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var p = repository.GetProductById(id);
            if (p == null)
            {
                return NotFound();
            }
            return p;
        }

        // POST: api/Products
        [HttpPost]
        public IActionResult PostProduct(Product p)
        {
            repository.SaveProduct(p);
            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var p = repository.GetProductById(id);
            if (p == null)
            {
                return NotFound();
            }
            repository.DeleteProduct(p);
            return NoContent();
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product p)
        {
            var pTmp = repository.GetProductById(id);
            if (pTmp == null)
            {
                return NotFound();
            }
            repository.UpdateProduct(p);
            return NoContent();
        }
    }
}
