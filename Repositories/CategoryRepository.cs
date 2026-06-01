using BusinessObjects.Models;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class CategoryRepository : ICatergoryRepository
    {
        public List<Category> GetAllCategories() => CategoryDAO.GetCategories();
    }
}
