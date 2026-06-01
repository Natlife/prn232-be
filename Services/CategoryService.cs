using BusinessObjects.Models;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICatergoryRepository iCategoryRepository;

        public CategoryService()
        {
            iCategoryRepository = new CategoryRepository();
        }

        public List<Category> GetCategories()
        {
            return iCategoryRepository.GetAllCategories();
        }
    }
}
