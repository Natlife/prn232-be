using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class PartCategoryDAO
    {
        private static PartCategoryDAO instance = null;
        private static readonly object instanceLock = new object();

        private PartCategoryDAO() { }

        public static PartCategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new PartCategoryDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<PartCategory> GetAllCategories()
        {
            using var context = new CarShowroomContext();
            return context.PartCategories.ToList();
        }

        public PartCategory? GetCategoryById(int id)
        {
            using var context = new CarShowroomContext();
            return context.PartCategories.SingleOrDefault(c => c.CategoryId == id);
        }
    }
}
