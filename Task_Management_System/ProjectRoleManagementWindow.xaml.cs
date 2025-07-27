using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.Service;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class ProjectRoleManagementWindow : Window
    {
        private readonly IProjectRoleService _projectRoleService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private Project _project;
        private ProjectRole _selectedProjectRole;

        public ProjectRoleManagementWindow(Project project)
        {
            InitializeComponent();
            _project = project;

            // Initialize services
            _projectRoleService = App.ServiceProvider.GetRequiredService<IProjectRoleService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _roleService = App.ServiceProvider.GetRequiredService<IRoleService>();

            LoadProjectInfo();
            LoadProjectRoles();
        }

        private void LoadProjectInfo()
        {
            txtProjectName.Text = _project.ProjectName;
        }

        private void LoadProjectRoles()
        {
            try
            {
                var projectRoles = _projectRoleService.GetByProjectId(_project.ProjectId).ToList();
                dgProjectRoles.ItemsSource = projectRoles;
                txtStatus.Text = $"Loaded {projectRoles.Count} project members";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project roles: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Error loading project roles";
            }
        }

        private void DgProjectRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProjectRole = dgProjectRoles.SelectedItem as ProjectRole;

            if (_selectedProjectRole != null)
            {
                // Enable action buttons
                btnChangeRole.IsEnabled = true;
                btnRemoveUser.IsEnabled = true;

                // Display member details
                txtUserName.Text = _selectedProjectRole.User?.FullName ?? "N/A";
                txtUserEmail.Text = _selectedProjectRole.User?.Email ?? "N/A";
                txtCurrentRole.Text = _selectedProjectRole.Role?.RoleName ?? "N/A";
            }
            else
            {
                // Disable action buttons
                btnChangeRole.IsEnabled = false;
                btnRemoveUser.IsEnabled = false;

                // Clear member details
                txtUserName.Text = "-";
                txtUserEmail.Text = "-";
                txtCurrentRole.Text = "-";
            }
        }

        private void BtnAddUserToProject_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserToProjectWindow(_project);
            if (addUserWindow.ShowDialog() == true)
            {
                LoadProjectRoles();
                txtStatus.Text = "User added to project successfully";
            }
        }

        private void BtnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProjectRole == null)
            {
                MessageBox.Show("Please select a project member to change role.", "No Selection",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var changeRoleWindow = new ChangeUserRoleWindow(_selectedProjectRole);
            if (changeRoleWindow.ShowDialog() == true)
            {
                LoadProjectRoles();
                txtStatus.Text = "User role changed successfully";
            }
        }

        private void BtnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProjectRole == null)
            {
                MessageBox.Show("Please select a project member to remove.", "No Selection",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to remove '{_selectedProjectRole.User?.FullName}' from this project?",
                                        "Confirm Remove", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _projectRoleService.RemoveUserFromProject(_selectedProjectRole.UserId, _project.ProjectId);
                    LoadProjectRoles();
                    txtStatus.Text = "User removed from project successfully";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing user from project: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}