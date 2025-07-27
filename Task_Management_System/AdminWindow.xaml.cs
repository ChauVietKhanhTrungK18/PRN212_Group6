using System.Windows;
using TMS_BLL.Service;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.IService;

namespace Task_Management_System
{
    public partial class AdminWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly INotificationService _notificationService;

        public AdminWindow()
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _notificationService = App.ServiceProvider.GetRequiredService<INotificationService>();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                var users = _userService.GetAll();
                txtTotalMembers.Text = users.Count(u => u.RoleId != 1).ToString();
                txtTotalProjects.Text = _projectService.GetAll().Count().ToString();
                txtTotalTasks.Text = _taskService.GetAll().Count().ToString();
                txtTotalNotifications.Text = _notificationService.GetAll().Count().ToString();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu Dashboard: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnManageAccounts_Click(object sender, RoutedEventArgs e)
        {
            var manageWindow = new ManageAccountsWindow
            {
                Owner = this
            };
            manageWindow.ShowDialog();
            LoadDashboard();
        }
    }
} 