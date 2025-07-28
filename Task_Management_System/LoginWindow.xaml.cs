using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TMS_BLL.IService;
using TMS_BLL.Service;

namespace Task_Management_System
{
    public partial class LoginWindow : Window
    {
        private readonly IUserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password!",
                                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _userService.Login(username, password);

            if (user == null)
            {
                MessageBox.Show("Invalid username or password!",
                                "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.IsDeleted)
            {
                MessageBox.Show("Your account has been deactivated. Please contact admin!",
                                "Access Denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            switch (user.RoleId)
            {
                case 1: 
                    var adminWindow = new AdminWindow();
                    adminWindow.Show();
                    break;

                case 2: 
                    var managerWindow = new ManagerWindow(user.UserId);
                    managerWindow.Show();
                    break;

                case 3:
                default:
                    var memberWindow = new MemberWindow();
                    memberWindow.Show();
                    break;
            }

            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegisterWindow();
            regWindow.ShowDialog();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
} 