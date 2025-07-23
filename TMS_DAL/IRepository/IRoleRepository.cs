using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface IRoleRepository
    {
        Role GetById(int roleId);
        Role GetByName(string roleName);
        IEnumerable<Role> GetAll();
        void Add(Role role);
        void Update(Role role);
        void Delete(int roleId);
    }
} 