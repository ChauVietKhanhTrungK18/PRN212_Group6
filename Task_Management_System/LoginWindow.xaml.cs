using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TMS_BLL.Service;

namespace Task_Management_System
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _userService.Login(username, password);
            if (user != null)
            {
                MainWindow main = new MainWindow(user.FullName);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegisterWindow();
            regWindow.ShowDialog();
        }
    }
} 