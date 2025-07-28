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

        private void LoadProjects()
        {
            dgProjects.ItemsSource = _projectService.GetProjectsByManager(_currentManagerId);  
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            var filteredProjects = _projectService.GetProjectsByManager(_currentManagerId)
                .Where(p => p.ProjectName.ToLower().Contains(searchText) || p.Description.ToLower().Contains(searchText))
                .ToList();
            dgProjects.ItemsSource = filteredProjects;
        }

        private void BtnManageProjects_Click(object sender, RoutedEventArgs e)
        {
            var manageProjectsWindow = new ManageProjectsWindow(_currentManagerId);
            manageProjectsWindow.ShowDialog();
        }

        private void BtnManageMembers_Click(object sender, RoutedEventArgs e)
        {
            var manageMembersWindow = new ManageMembersWindow(_currentManagerId);
            manageMembersWindow.ShowDialog();
        }

        private void BtnManageTasks_Click(object sender, RoutedEventArgs e)
        {
            var manageTasksWindow = new ManageTasksWindow(_currentManagerId);
            manageTasksWindow.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}