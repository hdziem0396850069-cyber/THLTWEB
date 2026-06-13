using System.Collections.Generic;
using System.Linq;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Chăm sóc da (Skincare)" },
                new Category { Id = 2, Name = "Son môi (Lipsticks)" },
                new Category { Id = 3, Name = "Trang điểm mắt (Eye Makeup)" },
                new Category { Id = 4, Name = "Nước hoa (Perfumes)" }
            };
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }

        public Category? GetById(int id)
        {
            return _categoryList.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            category.Id = _categoryList.Any() ? _categoryList.Max(c => c.Id) + 1 : 1;
            _categoryList.Add(category);
        }

        public void Update(Category category)
        {
            var index = _categoryList.FindIndex(c => c.Id == category.Id);
            if (index != -1)
            {
                _categoryList[index] = category;
            }
        }

        public void Delete(int id)
        {
            var category = _categoryList.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categoryList.Remove(category);
            }
        }
    }
}
