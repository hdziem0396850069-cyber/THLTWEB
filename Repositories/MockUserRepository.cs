using System.Collections.Generic;
using System.Linq;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public class MockUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public MockUserRepository()
        {
            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FullName = "Quản trị viên (Admin)",
                    Email = "admin@gmail.com",
                    Password = "123", // Supported passwords: '123' or '123456' or 'admin123'
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    FullName = "Khách hàng mẫu",
                    Email = "customer@gmail.com",
                    Password = "123",
                    Role = "Customer"
                }
            };
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User? GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));
        }

        public void Add(User user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
        }
    }
}
