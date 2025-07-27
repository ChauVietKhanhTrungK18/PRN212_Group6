using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetById(int userId)
            => _context.Users.FirstOrDefault(u => u.UserId == userId);

        public User GetByUsername(string username)
            => _context.Users.FirstOrDefault(u => u.Username == username);

        public User GetByEmail(string email)
            => _context.Users.FirstOrDefault(u => u.Email == email);

        public IEnumerable<User> GetAll(bool includeDeleted = false)
        {
            return includeDeleted
                ? _context.Users.ToList()
                : _context.Users.Where(u => !u.IsDeleted).ToList();
        }

        public IEnumerable<User> GetByRole(int roleId, bool includeDeleted = false)
        {
            return includeDeleted
                ? _context.Users.Where(u => u.RoleId == roleId).ToList()
                : _context.Users.Where(u => u.RoleId == roleId && !u.IsDeleted).ToList();
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void SetDeletedStatus(int userId, bool isDeleted)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.IsDeleted = isDeleted;
                _context.Users.Update(user);
            }
        }

        public bool ExistsByUsername(string username)
            => _context.Users.Any(u => u.Username == username && !u.IsDeleted);

        public bool ExistsByEmail(string email)
            => _context.Users.Any(u => u.Email == email && !u.IsDeleted);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
} 