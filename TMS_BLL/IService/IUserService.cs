using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface IUserService
    {
        bool Register(string username, string email, string password, string fullName);
        User Login(string username, string password);
        bool UpdateProfile(int userId, string fullName, string email, string password);
        User GetById(int userId);
        IEnumerable<User> GetAll();
        void Delete(int userId);
    }
} 