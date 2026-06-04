using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories
{
    public interface IPartCategoryRepository
    {
        IEnumerable<PartCategory> GetAllCategories();
        PartCategory? GetCategoryById(int id);
    }
}
