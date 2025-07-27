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
            txtTotalMembers.Text = _userService.GetAll().Count(u => !u.IsAdmin).ToString();
            txtTotalProjects.Text = _projectService.GetAll().Count().ToString();
            txtTotalTasks.Text = _taskService.GetAll().Count().ToString();
            //txtTotalNotifications.Text = _notificationService.GetAll().Count().ToString();
        }
    }
} 