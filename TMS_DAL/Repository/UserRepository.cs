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

        public User GetById(int userId) => _context.Users.Find(userId);
        public User GetByUsername(string username) => _context.Users.FirstOrDefault(u => u.Username == username);
        public User GetByEmail(string email) => _context.Users.FirstOrDefault(u => u.Email == email);
        public IEnumerable<User> GetAll() => _context.Users.ToList();
        public void Add(User user) { _context.Users.Add(user); _context.SaveChanges(); }
        public void Update(User user) { _context.Users.Update(user); _context.SaveChanges(); }
        public void Delete(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
} 