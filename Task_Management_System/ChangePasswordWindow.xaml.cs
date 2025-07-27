using System.Windows;
using TMS_BLL.Service;
using TMS_DAL.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Task_Management_System
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly UserService _userService;
        private User _currentUser;
        public ChangePasswordWindow(User user)
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
            _currentUser = user;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var oldPassword = txtOldPassword.Password;
            var newPassword = txtNewPassword.Password;
            var confirmPassword = txtConfirmPassword.Password;
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please enter all required information!");
                return;
            }
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New password and confirm password do not match!");
                return;
            }
            var result = _userService.ChangePassword(_currentUser.UserId, oldPassword, newPassword);
            if (result)
            {
                MessageBox.Show("Password changed successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Change password failed! Old password is incorrect.");
            }
        }
    }
} 