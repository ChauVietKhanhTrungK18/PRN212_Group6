using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TMS_BLL.IService;
using TMS_BLL.Service;

namespace Task_Management_System
{
    public partial class RegisterWindow : Window
    {
        private readonly IUserService _userService;

        public RegisterWindow()
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var fullName = txtFullName.Text.Trim();
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Password;
            var confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _userService.Register(username, email, password, fullName);
            if (result)
            {
                MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Username or Email already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 