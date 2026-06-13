using System.Collections.Generic;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }
}
