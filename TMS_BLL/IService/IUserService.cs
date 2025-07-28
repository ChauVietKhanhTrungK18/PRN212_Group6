using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface IUserService
    {
        bool Register(string username, string email, string password, string fullName);
        User Login(string username, string password);
        bool UpdateProfile(int userId, string fullName, string email);
        bool ChangePassword(int userId, string oldPassword, string newPassword);
        User GetById(int userId);
        IEnumerable<User> GetAll(bool includeDeleted = false);
        IEnumerable<User> GetByRole(int roleId, bool includeDeleted = false);
        bool SetDeletedStatus(int userId, bool isDeleted);
        bool UpdateRole(int userId, int roleId);
        void Update(User user);
    }
} 