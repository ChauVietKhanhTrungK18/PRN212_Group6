using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class TaskFormWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private ProjectTask _task;
        private bool _isEditMode;

        public TaskFormWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadProjects();
            SetDefaultValues();
        }

        public TaskFormWindow(ProjectTask task)
        {
            InitializeComponent();
            _task = task;
            _isEditMode = true;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadProjects();
            LoadTaskData();
        }

        private void LoadProjects()
        {
            try
            {
                var projects = _projectService.GetAll().ToList();
                cboProject.ItemsSource = projects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetDefaultValues()
        {
            txtHeader.Text = "Add New Task";
            cboStatus.SelectedIndex = 0; // Not Started
            dpDueDate.SelectedDate = DateTime.Today.AddDays(7);
        }

        private void LoadTaskData()
        {
            txtHeader.Text = "Edit Task";
            txtTaskName.Text = _task.TaskName;
            txtDescription.Text = _task.Description;
            dpDueDate.SelectedDate = _task.DueDate;
            // Set status
            for (int i = 0; i < cboStatus.Items.Count; i++)
            {
                var item = cboStatus.Items[i] as ComboBoxItem;
                if (item?.Content.ToString() == _task.Status)
                {
                    cboStatus.SelectedIndex = i;
                    break;
                }
            }
            // Set project
            if (_task.ProjectId > 0)
            {
                var projects = cboProject.ItemsSource as List<Project>;
                var project = projects?.FirstOrDefault(p => p.ProjectId == _task.ProjectId);
                if (project != null)
                {
                    cboProject.SelectedItem = project;
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
                    UpdateTask();
                }
                else
                {
                    CreateTask();
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateTask()
        {
            var task = new ProjectTask
            {
                TaskName = txtTaskName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                DueDate = dpDueDate.SelectedDate ?? DateTime.Today.AddDays(7),
                Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Not Started",
                ProjectId = (cboProject.SelectedItem as Project)?.ProjectId ?? 0
            };
            _taskService.Add(task);
        }

        private void UpdateTask()
        {
            _task.TaskName = txtTaskName.Text.Trim();
            _task.Description = txtDescription.Text.Trim();
            _task.DueDate = dpDueDate.SelectedDate ?? DateTime.Today.AddDays(7);
            _task.Status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Not Started";
            _task.ProjectId = (cboProject.SelectedItem as Project)?.ProjectId ?? 0;
            _taskService.Update(_task);
        }

        private bool ValidateForm()
        {
            txtValidation.Text = "";
            if (string.IsNullOrWhiteSpace(txtTaskName.Text))
            {
                txtValidation.Text = "Task name is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                txtValidation.Text = "Description is required.";
                return false;
            }
            if (!dpDueDate.SelectedDate.HasValue)
            {
                txtValidation.Text = "Due date is required.";
                return false;
            }
            if (cboProject.SelectedItem == null)
            {
                txtValidation.Text = "Project is required.";
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