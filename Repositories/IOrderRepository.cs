using System.Collections.Generic;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order? GetById(int id);
        Order? GetByCode(string code);
        void Add(Order order);
    }
}
