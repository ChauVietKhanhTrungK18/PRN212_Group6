using System.Windows;
using TMS_BLL.Service;
using TMS_DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;

namespace Task_Management_System
{
    public partial class MemberDashboardWindow : Window
    {
        private readonly UserService _userService;
        private readonly ProjectService _projectService;
        private readonly ProjectRoleService _projectRoleService;
        private User _currentUser;
        public MemberDashboardWindow(User user)
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
            _projectService = App.ServiceProvider.GetRequiredService<ProjectService>();
            _projectRoleService = App.ServiceProvider.GetRequiredService<ProjectRoleService>();
            _currentUser = user;
            HideAllPanels();
        }

        private void btnMyProjects_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            MyProjectsPanel.Visibility = Visibility.Visible;
            LoadMyProjectsSplit();
        }

        private void LoadMyProjectsSplit()
        {
            var roles = _projectRoleService.GetAll().Where(r => r.UserId == _currentUser.UserId).ToList();
            var managedIds = roles.Where(r => r.RoleId == 1).Select(r => r.ProjectId).Distinct().ToList();
            var participatedIds = roles.Where(r => r.RoleId != 1).Select(r => r.ProjectId).Distinct().ToList();
            var allProjects = _projectService.GetAll().ToList();
            dgManagedProjects.ItemsSource = allProjects.Where(p => managedIds.Contains(p.ProjectId)).ToList();
            dgParticipatedProjects.ItemsSource = allProjects.Where(p => participatedIds.Contains(p.ProjectId) && !managedIds.Contains(p.ProjectId)).ToList();
        }

        private void HideAllPanels()
        {
            MyProjectsPanel.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Collapsed;
        }

        // Các handler cho các nút quản lý dự án (chỉ hoạt động với Managed Projects)
        private void btnManageMembers_Click(object sender, RoutedEventArgs e)
        {
            var project = (sender as FrameworkElement).DataContext as Project;
            if (project != null)
            {
                // Mở cửa sổ quản lý thành viên cho project này
                var prw = new ProjectRoleWindow(project.ProjectId);
                prw.ShowDialog();
            }
        }
        private void btnAssignTask_Click(object sender, RoutedEventArgs e)
        {
            var project = (sender as FrameworkElement).DataContext as Project;
            if (project != null)
            {
                // Mở cửa sổ giao task cho project này
                // var assignTaskWindow = new AssignTaskWindow(project.ProjectId);
                // assignTaskWindow.ShowDialog();
            }
        }
        private void btnEditProject_Click(object sender, RoutedEventArgs e)
        {
            var project = (sender as FrameworkElement).DataContext as Project;
            if (project != null)
            {
                // Mở cửa sổ chỉnh sửa project này
                // var editProjectWindow = new EditProjectWindow(project.ProjectId);
                // editProjectWindow.ShowDialog();
            }
        }
        private void btnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            var project = (sender as FrameworkElement).DataContext as Project;
            if (project != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete project '{project.ProjectName}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _projectService.Delete(project.ProjectId);
                    LoadMyProjectsSplit();
                }
            }
        }
    }
} 