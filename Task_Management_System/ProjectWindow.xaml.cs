using System;
using System.Collections.Generic;
using System.Windows;
using TMS_BLL.Service;
using TMS_DAL.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Task_Management_System
{
    public partial class ProjectWindow : Window
    {
        private readonly ProjectService _projectService;
        private Project _selectedProject;
        public ProjectWindow()
        {
            InitializeComponent();
            _projectService = App.ServiceProvider.GetRequiredService<ProjectService>();
            LoadProjects();
        }

        private void LoadProjects()
        {
            var projects = _projectService.GetAll();
            dgProjects.ItemsSource = projects;
        }

        private void btnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            var projectName = txtProjectName.Text.Trim();
            var desc = txtDescription.Text.Trim();
            var start = dpStartDate.SelectedDate;
            var end = dpEndDate.SelectedDate;
            if (string.IsNullOrEmpty(projectName) || start == null || end == null)
            {
                MessageBox.Show("Please enter all required information!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var project = new Project
            {
                ProjectName = projectName,
                Description = desc,
                StartDate = start.Value,
                EndDate = end.Value
            };
            _projectService.Add(project);
            LoadProjects();
            ClearForm();
        }

        private void dgProjects_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedProject = dgProjects.SelectedItem as Project;
            if (_selectedProject != null)
            {
                txtProjectName.Text = _selectedProject.ProjectName;
                txtDescription.Text = _selectedProject.Description;
                dpStartDate.SelectedDate = _selectedProject.StartDate;
                dpEndDate.SelectedDate = _selectedProject.EndDate;
                btnUpdateProject.Visibility = Visibility.Visible;
                btnCreateProject.IsEnabled = false;
            }
            else
            {
                btnUpdateProject.Visibility = Visibility.Collapsed;
                btnCreateProject.IsEnabled = true;
            }
        }

        private void btnEditProject_Click(object sender, RoutedEventArgs e)
        {
            var project = (sender as FrameworkElement).DataContext as Project;
            if (project != null)
            {
                dgProjects.SelectedItem = project;
            }
        }

        private void btnUpdateProject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProject == null) return;
            var projectName = txtProjectName.Text.Trim();
            var desc = txtDescription.Text.Trim();
            var start = dpStartDate.SelectedDate;
            var end = dpEndDate.SelectedDate;
            if (string.IsNullOrEmpty(projectName) || start == null || end == null)
            {
                MessageBox.Show("Please enter all required information!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var updatedProject = new Project
            {
                ProjectId = _selectedProject.ProjectId,
                ProjectName = projectName,
                Description = desc,
                StartDate = start.Value,
                EndDate = end.Value,
                Status = _selectedProject.Status,
                DateCreated = _selectedProject.DateCreated,
                ManagerId = _selectedProject.ManagerId
            };
            _projectService.Update(updatedProject);
            LoadProjects();
            ClearForm();
        }

        private void btnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            var project = (sender as FrameworkElement).DataContext as Project;
            if (project != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete project '{project.ProjectName}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _projectService.Delete(project.ProjectId);
                    LoadProjects();
                    ClearForm();
                }
            }
        }

        private void ClearForm()
        {
            txtProjectName.Text = "";
            txtDescription.Text = "";
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            btnUpdateProject.Visibility = Visibility.Collapsed;
            btnCreateProject.IsEnabled = true;
            dgProjects.SelectedItem = null;
            _selectedProject = null;
        }
    }
} 