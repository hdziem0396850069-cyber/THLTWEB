using System.Collections.Generic;
using Webbanhang_TH02.Models;

namespace Webbanhang_TH02.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User? GetByEmail(string email);
        void Add(User user);
    }
}
