using System.Windows;
using TMS_BLL.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Task_Management_System
{
    public partial class AdminWindow : Window
    {
        private readonly UserService _userService;
        private readonly ProjectService _projectService;
        private readonly TaskService _taskService;
        private readonly NotificationService _notificationService;

        public AdminWindow()
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
            _projectService = App.ServiceProvider.GetRequiredService<ProjectService>();
            _taskService = App.ServiceProvider.GetRequiredService<TaskService>();
            _notificationService = App.ServiceProvider.GetRequiredService<NotificationService>();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            txtTotalMembers.Text = _userService.GetAll().Count(u => !u.IsAdmin).ToString();
            txtTotalProjects.Text = _projectService.GetAll().Count().ToString();
            txtTotalTasks.Text = _taskService.GetAll().Count().ToString();
            txtTotalNotifications.Text = _notificationService.GetAll().Count().ToString();
        }
    }
} 