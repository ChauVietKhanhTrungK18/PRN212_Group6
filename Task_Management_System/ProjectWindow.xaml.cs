using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TMS_BLL.IService;
using TMS_BLL.Service;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class ProjectWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IProjectRoleService _projectRoleService;
        private Project _selectedProject;
        public ProjectWindow()
        {
            InitializeComponent();
            // Initialize services
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _projectRoleService = App.ServiceProvider.GetRequiredService<IProjectRoleService>();

            LoadProjects();
        }

        private void LoadProjects()
        {
            try
            {
                var projects = _projectService.GetAll().ToList();
                dgProjects.ItemsSource = projects;
                txtStatus.Text = $"Loaded {projects.Count} projects";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Error loading projects";
            }
        }

        private void DgProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProject = dgProjects.SelectedItem as Project;

            if (_selectedProject != null)
            {
                // Enable action buttons
                btnEditProject.IsEnabled = true;
                btnDeleteProject.IsEnabled = true;
                btnManageRoles.IsEnabled = true;

                // Display project details
                txtProjectName.Text = _selectedProject.ProjectName;
                txtDescription.Text = _selectedProject.Description;
                txtStartDate.Text = _selectedProject.StartDate.ToString("dd/MM/yyyy");
                txtEndDate.Text = _selectedProject.EndDate.ToString("dd/MM/yyyy");
                txtProjectStatus.Text = _selectedProject.Status;
                txtManager.Text = _selectedProject.Manager?.FullName ?? "N/A";
            }
            else
            {
                // Disable action buttons
                btnEditProject.IsEnabled = false;
                btnDeleteProject.IsEnabled = false;
                btnManageRoles.IsEnabled = false;

                // Clear project details
                txtProjectName.Text = "-";
                txtDescription.Text = "-";
                txtStartDate.Text = "-";
                txtEndDate.Text = "-";
                txtProjectStatus.Text = "-";
                txtManager.Text = "-";
            }
        }

        private void BtnAddProject_Click(object sender, RoutedEventArgs e)
        {
            var projectForm = new ProjectFormWindow();
            if (projectForm.ShowDialog() == true)
            {
                LoadProjects();
                txtStatus.Text = "Project added successfully";
            }
        }

        private void BtnEditProject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProject == null)
            {
                MessageBox.Show("Please select a project to edit.", "No Selection",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var projectForm = new ProjectFormWindow(_selectedProject);
            if (projectForm.ShowDialog() == true)
            {
                LoadProjects();
                txtStatus.Text = "Project updated successfully";
            }
        }

        private void BtnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProject == null)
            {
                MessageBox.Show("Please select a project to delete.", "No Selection",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete project '{_selectedProject.ProjectName}'?",
                                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _projectService.Delete(_selectedProject.ProjectId);
                    LoadProjects();
                    txtStatus.Text = "Project deleted successfully";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting project: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnManageRoles_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProject == null)
            {
                MessageBox.Show("Please select a project to manage roles.", "No Selection",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var roleManagementWindow = new ProjectRoleManagementWindow(_selectedProject);
            roleManagementWindow.ShowDialog();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProjects();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}