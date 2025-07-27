using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;
using System.Windows;

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
            if (_userRepository.ExistsByUsername(username) || _userRepository.ExistsByEmail(email))
                return false;

            var user = new User
            {
                Username = username,
                Email = email,
                FullName = fullName,
                PasswordHash = HashPassword(password),
                DateCreated = DateTime.Now,
                RoleId = 3, // mặc định là Member
                IsDeleted = false
            };

            _userRepository.Add(user);
            _userRepository.SaveChanges();
            return true;
        }

        public User Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null) return null;

            return user.PasswordHash == HashPassword(password) ? user : null;
        }

        public bool UpdateProfile(int userId, string fullName, string email)
        {
            var user = _userRepository.GetById(userId);
            if (user == null || user.IsDeleted) return false;

            user.FullName = fullName;
            user.Email = email;

            _userRepository.Update(user);
            _userRepository.SaveChanges();
            return true;
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user == null || user.IsDeleted) return false;

            if (user.PasswordHash != HashPassword(oldPassword)) return false;

            user.PasswordHash = HashPassword(newPassword);
            _userRepository.Update(user);
            _userRepository.SaveChanges();
            return true;
        }

        public User GetById(int userId) => _userRepository.GetById(userId);

        public IEnumerable<User> GetAll(bool includeDeleted = false)
            => _userRepository.GetAll(includeDeleted);

        public IEnumerable<User> GetByRole(int roleId, bool includeDeleted = false)
            => _userRepository.GetByRole(roleId, includeDeleted);

        public bool SetDeletedStatus(int userId, bool isDeleted)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) return false;

            _userRepository.SetDeletedStatus(userId, isDeleted);
            _userRepository.SaveChanges();
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        public bool UpdateRole(int userId, int roleId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null || user.RoleId == 1) return false;
            user.RoleId = roleId;
            _userRepository.Update(user);
            _userRepository.SaveChanges();
            return true;
        }
    }
} 