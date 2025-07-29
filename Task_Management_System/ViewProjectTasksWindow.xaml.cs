using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class ViewProjectTasksWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly IUserService _userService;
        private readonly int _projectId;
        private Project _project;
        private List<ProjectTask> _allTasks;

        public ViewProjectTasksWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            
            // Initialize services
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskAssignmentService = App.ServiceProvider.GetRequiredService<ITaskAssignmentService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();

            LoadProjectInfo();
            LoadProjectTasks();
        }

        private void LoadProjectInfo()
        {
            try
            {
                _project = _projectService.GetById(_projectId);
                if (_project != null)
                {
                    txtProjectTitle.Text = $"Tasks - {_project.ProjectName}";
                    txtProjectInfo.Text = $"Project: {_project.ProjectName} | Status: {_project.Status} | Start: {_project.StartDate:dd/MM/yyyy} | End: {_project.EndDate:dd/MM/yyyy}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProjectTasks()
        {
            try
            {
                // Get all tasks for this project
                _allTasks = _taskService.GetTasksForProject(_projectId)?.ToList() ?? new List<ProjectTask>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _allTasks = new List<ProjectTask>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (dgProjectTasks == null || _allTasks == null)
                {
                    return;
                }

                var filteredTasks = _allTasks.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrEmpty(txtSearchTasks.Text))
                {
                    string searchTerm = txtSearchTasks.Text.ToLower();
                    filteredTasks = filteredTasks.Where(t => 
                        t.TaskName.ToLower().Contains(searchTerm) || 
                        t.Description.ToLower().Contains(searchTerm));
                }

                // Apply status filter
                if (cbTaskStatusFilter.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedStatus = selectedItem.Content.ToString();
                    if (selectedStatus != "All Tasks")
                    {
                        filteredTasks = filteredTasks.Where(t => t.Status == selectedStatus);
                    }
                }

                // Create anonymous objects with assigned members for display
                var tasksWithMembers = filteredTasks.Select(task =>
                {
                    var assignedMembers = _taskAssignmentService.GetAssignedMembers(task.TaskId);
                    var memberNames = assignedMembers?.Select(m => m.FullName).ToList() ?? new List<string>();
                    var assignedMembersString = memberNames.Any() ? string.Join(", ", memberNames) : "Unassigned";

                    return new
                    {
                        TaskId = task.TaskId,
                        TaskName = task.TaskName,
                        Description = task.Description,
                        Deadline = task.Deadline,
                        Status = task.Status,
                        DateCreated = task.DateCreated,
                        ProjectId = task.ProjectId,
                        AssignedMembers = assignedMembersString
                    };
                }).ToList();

                dgProjectTasks.ItemsSource = tasksWithMembers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchTasks_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbTaskStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProjectTasks();
        }

        private void BtnViewTaskDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjectTasks.SelectedItem != null)
            {
                try
                {
                    var selectedTask = dgProjectTasks.SelectedItem;
                    var taskName = selectedTask.GetType().GetProperty("TaskName")?.GetValue(selectedTask)?.ToString();
                    var description = selectedTask.GetType().GetProperty("Description")?.GetValue(selectedTask)?.ToString();
                    var status = selectedTask.GetType().GetProperty("Status")?.GetValue(selectedTask)?.ToString();
                    var deadline = selectedTask.GetType().GetProperty("Deadline")?.GetValue(selectedTask);
                    var assignedMembers = selectedTask.GetType().GetProperty("AssignedMembers")?.GetValue(selectedTask)?.ToString();

                    MessageBox.Show($"Task Details:\n\nName: {taskName}\nDescription: {description}\nStatus: {status}\nDeadline: {deadline:dd/MM/yyyy}\nAssigned To: {assignedMembers}", 
                        "Task Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing task details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a task to view details.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgProjectTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnViewTaskDetails_Click(sender, e);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 