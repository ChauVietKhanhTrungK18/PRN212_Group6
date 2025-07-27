using System.Windows;
using TMS_BLL.Service;
using TMS_DAL.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Task_Management_System
{
    public partial class UpdateProfileWindow : Window
    {
        private readonly UserService _userService;
        private User _currentUser;
        public UpdateProfileWindow(User user)
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
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
                MessageBox.Show("Please enter all required information!");
                return;
            }
            var result = _userService.UpdateProfile(_currentUser.UserId, fullName, email);
            if (result)
            {
                MessageBox.Show("Profile updated successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Update failed!");
            }
        }
    }
} 