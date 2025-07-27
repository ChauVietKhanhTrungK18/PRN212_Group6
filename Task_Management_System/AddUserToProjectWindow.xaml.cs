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
    public partial class AddUserToProjectWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private Project _project;

        public AddUserToProjectWindow(Project project)
        {
            InitializeComponent();
            _project = project;

            // Initialize services
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _roleService = App.ServiceProvider.GetRequiredService<IRoleService>();

            LoadProjectInfo();
            LoadData();
        }

        private void LoadProjectInfo()
        {
            txtProjectName.Text = _project.ProjectName;
        }

        private void LoadData()
        {
           
        }

        private void LstAvailableUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedUser = lstAvailableUsers.SelectedItem as User;
            if (selectedUser != null)
            {
                cboUser.SelectedItem = selectedUser;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
        }

        private bool ValidateForm()
        {
            txtValidation.Text = "";

            if (cboUser.SelectedItem == null)
            {
                txtValidation.Text = "Please select a user.";
                return false;
            }

            if (cboRole.SelectedItem == null)
            {
                txtValidation.Text = "Please select a role.";
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