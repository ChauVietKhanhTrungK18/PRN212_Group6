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
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private Project _project;

        public ProjectRoleManagementWindow(Project project)
        {
            InitializeComponent();
            _project = project;

            // Initialize services
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
        }

        private void DgProjectRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
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
           
        }

        private void BtnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}