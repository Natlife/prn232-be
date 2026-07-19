using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;

namespace Repositories
{
    public class PartCategoryRepository : IPartCategoryRepository
    {
        public IEnumerable<PartCategory> GetAllCategories() => PartCategoryDAO.Instance.GetAllCategories();

        public PartCategory? GetCategoryById(int id) => PartCategoryDAO.Instance.GetCategoryById(id);

        public void AddCategory(PartCategory category) => PartCategoryDAO.Instance.AddCategory(category);
    }
}
