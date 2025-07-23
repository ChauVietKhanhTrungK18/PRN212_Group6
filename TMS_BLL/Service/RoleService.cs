using System.Collections.Generic;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public Role GetById(int roleId) => _roleRepository.GetById(roleId);
        public Role GetByName(string roleName) => _roleRepository.GetByName(roleName);
        public IEnumerable<Role> GetAll() => _roleRepository.GetAll();
        public void Add(Role role) => _roleRepository.Add(role);
        public void Update(Role role) => _roleRepository.Update(role);
        public void Delete(int roleId) => _roleRepository.Delete(roleId);
    }
} 