using BusinessObjects.Models;
using System.Collections.Generic;

namespace Repositories
{
    public interface ICatergoryRepository
    {
        List<Category> GetAllCategories();
    }
}
