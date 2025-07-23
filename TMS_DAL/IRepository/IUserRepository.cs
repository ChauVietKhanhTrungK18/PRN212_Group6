using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface IUserRepository
    {
        User GetById(int userId);
        User GetByUsername(string username);
        User GetByEmail(string email);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(int userId);
    }
} 