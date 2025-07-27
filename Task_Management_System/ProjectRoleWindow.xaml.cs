using System.Linq;
using System.Windows;
using TMS_BLL.Service;
using TMS_DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Task_Management_System
{
    public partial class ProjectRoleWindow : Window
    {
        private readonly ProjectService _projectService;
        private readonly UserService _userService;
        private readonly ProjectRoleService _projectRoleService;
        private readonly RoleService _roleService;
        public ProjectRoleWindow()
        {
            InitializeComponent();
            _projectService = App.ServiceProvider.GetRequiredService<ProjectService>();
            _userService = App.ServiceProvider.GetRequiredService<UserService>();
            _projectRoleService = App.ServiceProvider.GetRequiredService<ProjectRoleService>();
            _roleService = App.ServiceProvider.GetRequiredService<RoleService>();
            LoadData();
        }

        private void LoadData()
        {
            cbProject.ItemsSource = _projectService.GetAll().ToList();
            cbUser.ItemsSource = _userService.GetAll().ToList();
            cbRole.ItemsSource = _roleService.GetAll().Where(r => r.RoleName == "Project Manager" || r.RoleName == "Member" || r.RoleName == "Viewer").ToList();
        }

        private void cbProject_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadProjectRoles();
        }

        private void LoadProjectRoles()
        {
            if (cbProject.SelectedValue == null) return;
            int projectId = (int)cbProject.SelectedValue;
            var roles = _projectRoleService.GetByProjectId(projectId).ToList();
            dgProjectRoles.ItemsSource = roles;
        }

        private void btnAssign_Click(object sender, RoutedEventArgs e)
        {
            if (cbProject.SelectedValue == null || cbUser.SelectedValue == null || cbRole.SelectedValue == null) return;
            int projectId = (int)cbProject.SelectedValue;
            int userId = (int)cbUser.SelectedValue;
            int roleId = (int)cbRole.SelectedValue;
            // Đảm bảo 1 dự án chỉ có 1 Project Manager
            var currentRoles = _projectRoleService.GetByProjectId(projectId);
            if (roleId == 1 && currentRoles.Any(r => r.RoleId == 1))
            {
                MessageBox.Show("Each project can only have one Project Manager!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Kiểm tra user đã có vai trò này chưa
            if (currentRoles.Any(r => r.UserId == userId && r.RoleId == roleId))
            {
                MessageBox.Show("This user already has this role in the project!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var pr = new ProjectRole { ProjectId = projectId, UserId = userId, RoleId = roleId };
            _projectRoleService.Add(pr);
            LoadProjectRoles();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var pr = (sender as FrameworkElement).DataContext as ProjectRole;
            if (pr != null)
            {
                _projectRoleService.Delete(pr.ProjectRoleId);
                LoadProjectRoles();
            }
        }
    }
} 