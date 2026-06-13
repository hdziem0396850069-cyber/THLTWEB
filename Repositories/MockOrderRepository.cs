using System.Collections.Generic;
using System.Linq;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public class MockOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders;

        public MockOrderRepository()
        {
            _orders = new List<Order>();
        }

        public IEnumerable<Order> GetAll()
        {
            return _orders;
        }

        public Order? GetById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public Order? GetByCode(string code)
        {
            return _orders.FirstOrDefault(o => o.OrderCode.Equals(code, System.StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Order order)
        {
            order.Id = _orders.Any() ? _orders.Max(o => o.Id) + 1 : 1;
            _orders.Add(order);
        }
    }
}
