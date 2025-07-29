using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly int _currentManagerId;
        private List<Project> _allProjects;

        public ManagerWindow(int managerId)
        {
            InitializeComponent();
            _currentManagerId = managerId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();

            LoadDashboard();
            LoadProjects();
        }

        private void LoadDashboard()
        {
            try
            {
                // Sử dụng _currentManagerId thay vì giả sử ID
                var managedProjects = _projectService.GetProjectsByManager(_currentManagerId);
                txtTotalProjects.Text = managedProjects.Count().ToString();

                var totalTasks = _taskService.GetAll()
                                             .Where(t => managedProjects.Any(p => p.ProjectId == t.ProjectId))
                                             .Count();
                txtTotalTasks.Text = totalTasks.ToString();

                var totalMembers = _projectMemberService.GetProjectMembersForManager(_currentManagerId).Count();
                txtTotalMembers.Text = totalMembers.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProjects()
        {
            try
            {
                _allProjects = _projectService.GetProjectsByManager(_currentManagerId)?.ToList() ?? new List<Project>();

                if (_allProjects.Count == 0)
                {
                    MessageBox.Show("No projects found for this manager.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    dgProjects.ItemsSource = null;
                }
                else
                {
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (dgProjects == null)
                {
                    return;
                }

                if (_allProjects == null || !_allProjects.Any())
                {
                    dgProjects.ItemsSource = null;
                    return;
                }

                var filteredProjects = _allProjects.AsQueryable();

                // Apply search filter
                string searchText = txtSearch?.Text?.Trim().ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    filteredProjects = filteredProjects.Where(p =>
                        p.ProjectName.ToLower().Contains(searchText) ||
                        p.Description.ToLower().Contains(searchText));
                }

                // Apply status filter
                if (cbStatusFilter != null && cbStatusFilter.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedStatus = selectedItem.Content.ToString();
                    if (selectedStatus != "All Projects")
                    {
                        filteredProjects = filteredProjects.Where(p => p.Status == selectedStatus);
                    }
                }

                dgProjects.ItemsSource = filteredProjects.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var createProjectWindow = new CreateProjectWindow(_currentManagerId);
                createProjectWindow.ShowDialog();
                LoadProjects();
                LoadDashboard(); // Refresh dashboard stats
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditProject_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is Project selectedProject)
            {
                try
                {
                    var editProjectWindow = new EditProjectWindow(selectedProject.ProjectId);
                    editProjectWindow.ShowDialog();
                    LoadProjects();
                    LoadDashboard(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error editing project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a project to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is Project selectedProject)
            {
                var confirmation = MessageBox.Show(
                    $"Are you sure you want to delete project '{selectedProject.ProjectName}'?\n\nThis action will also delete all related tasks and members.", 
                    "Delete Project", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                
                if (confirmation == MessageBoxResult.Yes)
                {
                    try
                    {
                        _projectService.Delete(selectedProject.ProjectId);
                        MessageBox.Show("Project deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProjects();
                        LoadDashboard(); // Refresh dashboard stats
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a project to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is Project selectedProject)
            {
                try
                {
                    var projectDetailWindow = new ProjectDetailWindow(selectedProject.ProjectId);
                    projectDetailWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing project details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a project to view details.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnManageMembers_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem != null)
            {
                var selectedProject = dgProjects.SelectedItem as Project; 
                var selectedProjectId = selectedProject?.ProjectId;

                if (selectedProjectId.HasValue)
                {
                    var manageMembersWindow = new ManageMembersWindow(_currentManagerId, selectedProjectId.Value);
                    manageMembersWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please select a project first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a project from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnManageTasks_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem != null)
            {
                var selectedProject = dgProjects.SelectedItem as Project; 
                var selectedProjectId = selectedProject?.ProjectId;

                if (selectedProjectId.HasValue)
                {
                    var manageTasksWindow = new ManageTasksWindow(selectedProjectId.Value);
                    manageTasksWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please select a project first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a project from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProjects();
            LoadDashboard();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}