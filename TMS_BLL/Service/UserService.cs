using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Register(string username, string email, string password, string fullName)
        {
            if (_userRepository.GetByUsername(username) != null || _userRepository.GetByEmail(email) != null)
                return false; 

            var user = new User
            {
                Username = username,
                Email = email,
                FullName = fullName,
                PasswordHash = HashPassword(password),
                DateCreated = DateTime.Now
            };
            _userRepository.Add(user);
            return true;
        }

        public User Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null) return null;
            if (user.PasswordHash == HashPassword(password))
                return user;
            return null;
        }

        public bool UpdateProfile(int userId, string fullName, string email, string password)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) return false;
            user.FullName = fullName;
            user.Email = email;
            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = HashPassword(password);
            _userRepository.Update(user);
            return true;
        }

        public User GetById(int userId) => _userRepository.GetById(userId);
        public IEnumerable<User> GetAll() => _userRepository.GetAll();
        public void Delete(int userId) => _userRepository.Delete(userId);

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
} 