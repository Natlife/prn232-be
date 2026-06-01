using BusinessObjects.Models;
using System.Collections.Generic;

namespace Repositories
{
    public interface IProductRepository
    {
        void SaveProduct(Product p);
        Product GetProductById(int id);
        void DeleteProduct(Product p);
        void UpdateProduct(Product p);
        List<Product> GetProducts();
    }
}
