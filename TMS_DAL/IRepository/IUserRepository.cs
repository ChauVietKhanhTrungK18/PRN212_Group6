using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface IUserRepository
    {
        User GetById(int userId);
        User GetByUsername(string username);
        User GetByEmail(string email);
        IEnumerable<User> GetAll(bool includeDeleted = false);
        IEnumerable<User> GetByRole(int roleId, bool includeDeleted = false);
        void Add(User user);
        void Update(User user);
        void SetDeletedStatus(int userId, bool isDeleted);
        bool ExistsByUsername(string username);
        bool ExistsByEmail(string email);
        void SaveChanges();
    }
} 