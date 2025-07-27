using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.Service;
using TMS_BLL.IService;
using TMS_DAL.Model;
using System.Windows.Controls;

namespace Task_Management_System
{
    public partial class ProjectFormWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private Project _project;
        private bool _isEditMode;

        public ProjectFormWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            LoadUsers();
            SetDefaultValues();
        }

        public ProjectFormWindow(Project project)
        {
            InitializeComponent();
            _project = project;
            _isEditMode = true;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            LoadUsers();
            LoadProjectData();
        }

        private void LoadUsers()
        {
            try
            {
                var users = _userService.GetAll().ToList();
                cboManager.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetDefaultValues()
        {
            txtHeader.Text = "Add New Project";
            cboStatus.SelectedIndex = 0; // Planning
            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today.AddMonths(1);
        }

        private void LoadProjectData()
        {
            txtHeader.Text = "Edit Project";
            txtProjectName.Text = _project.ProjectName;
            txtDescription.Text = _project.Description;
            dpStartDate.SelectedDate = _project.StartDate;
            dpEndDate.SelectedDate = _project.EndDate;

            // Set status
            for (int i = 0; i < cboStatus.Items.Count; i++)
            {
                var item = cboStatus.Items[i] as ComboBoxItem;
                if (item?.Content.ToString() == _project.Status)
                {
                    cboStatus.SelectedIndex = i;
                    break;
                }
            }

            // Set manager
            if (_project.ManagerId > 0)
            {
                var users = cboManager.ItemsSource as List<User>;
                var manager = users?.FirstOrDefault(u => u.UserId == _project.ManagerId);
                if (manager != null)
                {
                    cboManager.SelectedItem = manager;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                if (_isEditMode)
                {
                    UpdateProject();
                }
                else
                {
                    CreateProject();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving project: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateProject()
        {
            var project = new Project
            {
                ProjectName = txtProjectName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                StartDate = dpStartDate.SelectedDate ?? DateTime.Today,
                EndDate = dpEndDate.SelectedDate ?? DateTime.Today.AddMonths(1),
                Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Planning",
                DateCreated = DateTime.Now,
                ManagerId = (cboManager.SelectedItem as User)?.UserId ?? 0
            };

            _projectService.Add(project);
        }

        private void UpdateProject()
        {
            _project.ProjectName = txtProjectName.Text.Trim();
            _project.Description = txtDescription.Text.Trim();
            _project.StartDate = dpStartDate.SelectedDate ?? DateTime.Today;
            _project.EndDate = dpEndDate.SelectedDate ?? DateTime.Today.AddMonths(1);
            _project.Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Planning";
            _project.ManagerId = (cboManager.SelectedItem as User)?.UserId ?? 0;

            _projectService.Update(_project);
        }

        private bool ValidateForm()
        {
            txtValidation.Text = "";

            if (string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                txtValidation.Text = "Project name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                txtValidation.Text = "Description is required.";
                return false;
            }

            if (!dpStartDate.SelectedDate.HasValue)
            {
                txtValidation.Text = "Start date is required.";
                return false;
            }

            if (!dpEndDate.SelectedDate.HasValue)
            {
                txtValidation.Text = "End date is required.";
                return false;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                txtValidation.Text = "Start date cannot be after end date.";
                return false;
            }

            if (cboManager.SelectedItem == null)
            {
                txtValidation.Text = "Project manager is required.";
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