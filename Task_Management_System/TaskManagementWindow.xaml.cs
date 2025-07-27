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
    public partial class TaskManagementWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private ProjectTask _selectedTask;

        public TaskManagementWindow()
        {
            InitializeComponent();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadTasks();
        }

        private void LoadTasks()
        {
            try
            {
                var tasks = _taskService.GetAll().ToList();
                dgTasks.ItemsSource = tasks;
                txtStatusBar.Text = $"Loaded {tasks.Count} tasks";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatusBar.Text = "Error loading tasks";
            }
        }

        private void DgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTask = dgTasks.SelectedItem as ProjectTask;
            if (_selectedTask != null)
            {
                btnEditTask.IsEnabled = true;
                btnDeleteTask.IsEnabled = true;
                btnAssignTask.IsEnabled = true;
                btnUpdateStatus.IsEnabled = true;
                txtTaskName.Text = _selectedTask.TaskName;
                txtDescription.Text = _selectedTask.Description;
                txtDueDate.Text = _selectedTask.DueDate.ToString("dd/MM/yyyy");
                txtStatus.Text = _selectedTask.Status;
                txtAssignee.Text = _selectedTask.Assignee?.FullName ?? "-";
                txtProject.Text = _selectedTask.Project?.ProjectName ?? "-";
            }
            else
            {
                btnEditTask.IsEnabled = false;
                btnDeleteTask.IsEnabled = false;
                btnAssignTask.IsEnabled = false;
                btnUpdateStatus.IsEnabled = false;
                txtTaskName.Text = "-";
                txtDescription.Text = "-";
                txtDueDate.Text = "-";
                txtStatus.Text = "-";
                txtAssignee.Text = "-";
                txtProject.Text = "-";
            }
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            var taskForm = new TaskFormWindow();
            if (taskForm.ShowDialog() == true)
            {
                LoadTasks();
                txtStatusBar.Text = "Task added successfully";
            }
        }

        private void BtnEditTask_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Please select a task to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var taskForm = new TaskFormWindow(_selectedTask);
            if (taskForm.ShowDialog() == true)
            {
                LoadTasks();
                txtStatusBar.Text = "Task updated successfully";
            }
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Please select a task to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete task '{_selectedTask.TaskName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _taskService.Delete(_selectedTask.TaskId);
                    LoadTasks();
                    txtStatusBar.Text = "Task deleted successfully";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAssignTask_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Please select a task to assign.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var assignWindow = new AssignTaskWindow(_selectedTask);
            if (assignWindow.ShowDialog() == true)
            {
                LoadTasks();
                txtStatusBar.Text = "Task assigned successfully";
            }
        }

        private void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Please select a task to update status.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var updateStatusWindow = new UpdateTaskStatusWindow(_selectedTask);
            if (updateStatusWindow.ShowDialog() == true)
            {
                LoadTasks();
                txtStatusBar.Text = "Task status updated successfully";
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 