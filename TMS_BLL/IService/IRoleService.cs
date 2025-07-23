using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface IRoleService
    {
        Role GetById(int roleId);
        Role GetByName(string roleName);
        IEnumerable<Role> GetAll();
        void Add(Role role);
        void Update(Role role);
        void Delete(int roleId);
    }
} 