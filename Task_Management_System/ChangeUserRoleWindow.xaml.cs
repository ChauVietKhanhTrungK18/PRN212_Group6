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
    public partial class ChangeUserRoleWindow : Window
    {
        private readonly IRoleService _roleService;

        public ChangeUserRoleWindow()
        {
            InitializeComponent();

            // Initialize services
            _roleService = App.ServiceProvider.GetRequiredService<IRoleService>();

            LoadData();
        }

        private void LoadData()
        {
        }

        private void CboNewRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRole = cboNewRole.SelectedItem as Role;
            if (selectedRole != null)
            {
                txtRoleDescription.Text = selectedRole.Description ?? "No description available.";
            }
            else
            {
                txtRoleDescription.Text = "Role description will appear here";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private bool ValidateForm()
        {
            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}