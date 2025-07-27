using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.Service;
using TMS_DAL.Model;
using TMS_BLL.IService;

namespace Task_Management_System
{
    public partial class AddUserToProjectWindow : Window
    {
        private readonly IProjectRoleService _projectRoleService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private Project _project;

        public AddUserToProjectWindow(Project project)
        {
            InitializeComponent();
            _project = project;

            // Initialize services
            _projectRoleService = App.ServiceProvider.GetRequiredService<IProjectRoleService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _roleService = App.ServiceProvider.GetRequiredService<IRoleService>();

            LoadProjectInfo();
            LoadData();
        }

        private void LoadProjectInfo()
        {
            txtProjectName.Text = _project.ProjectName;
        }

        private void LoadData()
        {
            try
            {
                // Load all users
                var allUsers = _userService.GetAll().ToList();

                // Get users already in the project
                var projectUsers = _projectRoleService.GetByProjectId(_project.ProjectId)
                    .Select(pr => pr.UserId)
                    .ToList();

                // Filter out users already in the project
                var availableUsers = allUsers.Where(u => !projectUsers.Contains(u.UserId)).ToList();

                lstAvailableUsers.ItemsSource = availableUsers;
                cboUser.ItemsSource = availableUsers;

                // Load roles
                var roles = _roleService.GetAll().ToList();
                cboRole.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LstAvailableUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedUser = lstAvailableUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                cboUser.SelectedItem = selectedUser;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var selectedUser = cboUser.SelectedItem as User;
                var selectedRole = cboRole.SelectedItem as Role;

                var projectRole = new ProjectRole
                {
                    UserId = selectedUser.UserId,
                    ProjectId = _project.ProjectId,
                    RoleId = selectedRole.RoleId
                };

                _projectRoleService.Add(projectRole);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user to project: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            txtValidation.Text = "";

            if (cboUser.SelectedItem == null)
            {
                txtValidation.Text = "Please select a user.";
                return false;
            }

            if (cboRole.SelectedItem == null)
            {
                txtValidation.Text = "Please select a role.";
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}