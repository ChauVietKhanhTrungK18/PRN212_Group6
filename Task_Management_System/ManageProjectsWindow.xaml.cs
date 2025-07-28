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
    /// Interaction logic for ManageProjectsWindow.xaml
    /// </summary>
    public partial class ManageProjectsWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly int _currentManagerId;
        private List<Project> _allProjects;

        public ManageProjectsWindow(int managerId)
        {
            InitializeComponent();
            _currentManagerId = managerId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadProjects();
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
                if (_allProjects == null || !_allProjects.Any())
                {
                    MessageBox.Show("No projects available to filter.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;  
                }

                var filteredProjects = _allProjects.AsQueryable();

                string searchText = txtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    filteredProjects = filteredProjects.Where(p =>
                        p.ProjectName.ToLower().Contains(searchText) ||
                        p.Description.ToLower().Contains(searchText));
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

        //private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // Kiểm tra nếu người dùng đã chọn giá trị hợp lệ
        //    if (cbStatusFilter.SelectedItem == null)
        //    {
        //        return;  // Nếu chưa có lựa chọn hợp lệ thì không làm gì cả
        //    }

        //    // Lấy ComboBoxItem được chọn
        //    var selectedItem = cbStatusFilter.SelectedItem as ComboBoxItem;

        //    if (selectedItem != null)
        //    {
        //        string selectedStatus = selectedItem.Content.ToString();

        //        // Kiểm tra nếu chọn "All Status", không áp dụng bộ lọc
        //        if (selectedStatus == "All Status")
        //        {
        //            dgProjects.ItemsSource = _allProjects;  // Hiển thị tất cả các dự án
        //        }
        //        else
        //        {
        //            ApplyFilters();  // Áp dụng bộ lọc với trạng thái đã chọn
        //        }
        //    }
        //}
        private void BtnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var createProjectWindow = new CreateProjectWindow(_currentManagerId);
                createProjectWindow.ShowDialog();
                LoadProjects();
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

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProjects();
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
    }
}
