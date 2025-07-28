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

        public ManageProjectsWindow(int managerId)
        {
            InitializeComponent();
            _currentManagerId = managerId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();

            LoadProjects();
        }

        private void LoadProjects()
        {
            dgProjects.ItemsSource = _projectService.GetProjectsByManager(_currentManagerId);  // Lấy tất cả các dự án của Manager hiện tại
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            var filteredProjects = _projectService.GetProjectsByManager(_currentManagerId)
                .Where(p => p.ProjectName.ToLower().Contains(searchText) || p.Description.ToLower().Contains(searchText))
                .ToList();
            dgProjects.ItemsSource = filteredProjects;
        }

        private void BtnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            var createProjectWindow = new CreateProjectWindow(_currentManagerId);
            createProjectWindow.ShowDialog();
            LoadProjects();  
        }

        private void BtnEditProject_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is Project selectedProject)
            {
                var editProjectWindow = new EditProjectWindow(selectedProject.ProjectId);
                editProjectWindow.ShowDialog();
                LoadProjects();  
            }
        }

        private void BtnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjects.SelectedItem is Project selectedProject)
            {
                var confirmation = MessageBox.Show($"Are you sure you want to delete project '{selectedProject.ProjectName}'?", "Delete Project", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmation == MessageBoxResult.Yes)
                {
                    _projectService.Delete(selectedProject.ProjectId);
                    LoadProjects(); 
                }
            }
        }
    }
}
