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
    public partial class ChangeUserRoleWindow : Window
    {
        private readonly IProjectRoleService _projectRoleService;
        private readonly IRoleService _roleService;
        private ProjectRole _projectRole;

        public ChangeUserRoleWindow(ProjectRole projectRole)
        {
            InitializeComponent();
            _projectRole = projectRole;

            // Initialize services
            _projectRoleService = App.ServiceProvider.GetRequiredService<IProjectRoleService>();
            _roleService = App.ServiceProvider.GetRequiredService<IRoleService>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Display user and current role info
                txtUserName.Text = _projectRole.User?.FullName ?? "N/A";
                txtCurrentRole.Text = _projectRole.Role?.RoleName ?? "N/A";

                // Load all available roles
                var roles = _roleService.GetAll().ToList();
                cboNewRole.ItemsSource = roles;

                // Set current role as selected
                if (_projectRole.Role != null)
                {
                    var currentRole = roles.FirstOrDefault(r => r.RoleId == _projectRole.RoleId);
                    if (currentRole != null)
                    {
                        cboNewRole.SelectedItem = currentRole;
                    }
                }

                // Add event handler for role selection change
                cboNewRole.SelectionChanged += CboNewRole_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CboNewRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRole = cboNewRole.SelectedItem as Role;
            if (selectedRole != null)
            {
                txtRoleDescription.Text = selectedRole.Description ?? "No description available.";
            }
            else
            {
                txtRoleDescription.Text = "Role description will appear here";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var selectedRole = cboNewRole.SelectedItem as Role;

                // Update the project role
                _projectRole.RoleId = selectedRole.RoleId;
                _projectRoleService.Update(_projectRole);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user role: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            txtValidation.Text = "";

            if (cboNewRole.SelectedItem == null)
            {
                txtValidation.Text = "Please select a new role.";
                return false;
            }

            var selectedRole = cboNewRole.SelectedItem as Role;
            if (selectedRole.RoleId == _projectRole.RoleId)
            {
                txtValidation.Text = "Please select a different role.";
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