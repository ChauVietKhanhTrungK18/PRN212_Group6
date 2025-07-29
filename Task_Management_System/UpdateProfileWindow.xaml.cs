using System.Windows;
using TMS_BLL.Service;
using TMS_DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.IService;

namespace Task_Management_System
{
    public partial class UpdateProfileWindow : Window
    {
        private readonly IUserService _userService;
        private User _currentUser;
        public UpdateProfileWindow(User user)
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _currentUser = user;
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var fullName = txtFullName.Text.Trim();
            var email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter all required information!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = _userService.UpdateProfile(_currentUser.UserId, fullName, email);
            if (result)
            {
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Update failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 