using System.Collections.Generic;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        Category? GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }
}