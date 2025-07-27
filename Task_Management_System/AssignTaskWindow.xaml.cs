using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class AssignTaskWindow : Window
    {
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private ProjectTask _task;

        public AssignTaskWindow(ProjectTask task)
        {
            InitializeComponent();
            _task = task;
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                var project = _projectService.GetById(_task.ProjectId);
                var users = project.ProjectRoles.Select(pr => pr.User).ToList();
                cboUser.ItemsSource = users;
                if (_task.AssigneeId.HasValue)
                {
                    var user = users.FirstOrDefault(u => u.UserId == _task.AssigneeId);
                    if (user != null) cboUser.SelectedItem = user;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAssign_Click(object sender, RoutedEventArgs e)
        {
            txtValidation.Text = "";
            if (cboUser.SelectedItem == null)
            {
                txtValidation.Text = "Please select a user.";
                return;
            }
            var user = cboUser.SelectedItem as User;
            _task.AssigneeId = user.UserId;
            _taskService.Update(_task);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 