using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Role GetById(int roleId) => _context.Roles.Find(roleId);
        public Role GetByName(string roleName) => _context.Roles.FirstOrDefault(r => r.RoleName == roleName);
        public IEnumerable<Role> GetAll() => _context.Roles.ToList();
        public void Add(Role role) { _context.Roles.Add(role); _context.SaveChanges(); }
        public void Update(Role role) { _context.Roles.Update(role); _context.SaveChanges(); }
        public void Delete(int roleId)
        {
            var role = _context.Roles.Find(roleId);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
            }
        }
    }
} 