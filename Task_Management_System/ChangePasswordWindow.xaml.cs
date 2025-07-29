using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly IUserService _userService;
        private readonly int _userId;
        private User _currentUser;

        public ChangePasswordWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                _currentUser = _userService.GetById(_userId);
                if (_currentUser == null)
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentPassword = txtCurrentPassword.Password;
                string newPassword = txtNewPassword.Password;
                string confirmPassword = txtConfirmPassword.Password;

                // Validation
                if (string.IsNullOrEmpty(currentPassword))
                {
                    MessageBox.Show("Please enter your current password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCurrentPassword.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show("Please enter a new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNewPassword.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(confirmPassword))
                {
                    MessageBox.Show("Please confirm your new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtConfirmPassword.Focus();
                    return;
                }

                // Check if current password is correct
                if (_currentUser.PasswordHash != HashPassword(currentPassword))
                {
                    MessageBox.Show("Current password is incorrect.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCurrentPassword.Clear();
                    txtCurrentPassword.Focus();
                    return;
                }

                // Check if new password matches confirmation
                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("New password and confirmation do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                    txtNewPassword.Focus();
                    return;
                }

                //// Validate password strength
                //if (!IsPasswordStrong(newPassword))
                //{
                //    MessageBox.Show("Password does not meet the requirements. Please ensure your password:\n\n• Is at least 8 characters long\n• Contains at least one uppercase letter\n• Contains at least one lowercase letter\n• Contains at least one number\n• Contains at least one special character", 
                //        "Password Requirements", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    txtNewPassword.Clear();
                //    txtConfirmPassword.Clear();
                //    txtNewPassword.Focus();
                //    return;
                //}

                // Check if new password is same as current
                if (HashPassword(newPassword) == _currentUser.PasswordHash)
                {
                    MessageBox.Show("New password must be different from current password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                    txtNewPassword.Focus();
                    return;
                }

                // Update password
                _currentUser.PasswordHash = HashPassword(newPassword);
                _userService.Update(_currentUser);

                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private bool IsPasswordStrong(string password)
        {
            // Check length
            if (password.Length < 8)
                return false;

            // Check for uppercase letter
            if (!password.Any(char.IsUpper))
                return false;

            // Check for lowercase letter
            if (!password.Any(char.IsLower))
                return false;

            // Check for number
            if (!password.Any(char.IsDigit))
                return false;

            // Check for special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
                return false;

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 