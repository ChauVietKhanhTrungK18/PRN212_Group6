using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for ManageTasksWindow.xaml
    /// </summary>
    public partial class ManageTasksWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly int _projectId;
        private Project _project;
        private List<ProjectTask> _allTasks;

        public ManageTasksWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskAssignmentService = App.ServiceProvider.GetRequiredService<ITaskAssignmentService>();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _project = _projectService.GetById(_projectId);
                if (_project != null)
                {
                    txtProjectInfo.Text = _project.ProjectName;
                }

                _allTasks = _taskService.GetTasksForProject(_projectId)?.ToList() ?? new List<ProjectTask>();
                
                // Create dynamic objects with assigned members information
                var taskDisplayList = new List<dynamic>();
                foreach (var task in _allTasks)
                {
                    string assignedMembersString = "N/A";
                    try
                    {
                        var assignedMembers = _taskAssignmentService.GetAssignedMembers(task.TaskId);
                        assignedMembersString = string.Join(", ", assignedMembers.Select(m => m.FullName));
                    }
                    catch (Exception ex)
                    {
                        // If there's a database column error, show N/A for assigned members
                        assignedMembersString = "N/A (Database Error)";
                        System.Diagnostics.Debug.WriteLine($"Error getting assigned members for task {task.TaskId}: {ex.Message}");
                    }
                    
                    taskDisplayList.Add(new
                    {
                        TaskId = task.TaskId,
                        TaskName = task.TaskName,
                        Description = task.Description,
                        Deadline = task.Deadline,
                        Status = task.Status,
                        DateCreated = task.DateCreated,
                        AssignedMembers = assignedMembersString
                    });
                }
                
                dgTasks.ItemsSource = taskDisplayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (dgTasks == null)
            {
                return;
            }

            var filteredTasks = _allTasks?.AsEnumerable() ?? Enumerable.Empty<ProjectTask>();

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                filteredTasks = filteredTasks.Where(t =>
                    t.TaskName.ToLower().Contains(searchTerm) ||
                    t.Description.ToLower().Contains(searchTerm));
            }

            // Create dynamic objects with assigned members information
            var taskDisplayList = new List<dynamic>();
            foreach (var task in filteredTasks)
            {
                string assignedMembersString = "N/A";
                try
                {
                    var assignedMembers = _taskAssignmentService.GetAssignedMembers(task.TaskId);
                    assignedMembersString = string.Join(", ", assignedMembers.Select(m => m.FullName));
                }
                catch (Exception ex)
                {
                    // If there's a database column error, show N/A for assigned members
                    assignedMembersString = "N/A (Database Error)";
                    System.Diagnostics.Debug.WriteLine($"Error getting assigned members for task {task.TaskId}: {ex.Message}");
                }
                
                taskDisplayList.Add(new
                {
                    TaskId = task.TaskId,
                    TaskName = task.TaskName,
                    Description = task.Description,
                    Deadline = task.Deadline,
                    Status = task.Status,
                    DateCreated = task.DateCreated,
                    AssignedMembers = assignedMembersString
                });
            }

            dgTasks.ItemsSource = taskDisplayList;
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnCreateTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var createTaskWindow = new CreateTaskWindow(_projectId);
                createTaskWindow.ShowDialog();
                LoadData(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgTasks.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    // Get TaskId from dynamic object
                    int taskId = (int)selectedItem.GetType().GetProperty("TaskId").GetValue(selectedItem);
                    var editTaskWindow = new EditTaskWindow(taskId);
                    editTaskWindow.ShowDialog();
                    LoadData(); // Refresh the task list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error editing task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a task to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgTasks.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    // Get task information from dynamic object
                    int taskId = (int)selectedItem.GetType().GetProperty("TaskId").GetValue(selectedItem);
                    string taskName = (string)selectedItem.GetType().GetProperty("TaskName").GetValue(selectedItem);

                    var confirmation = MessageBox.Show(
                        $"Are you sure you want to delete task '{taskName}'?\n\nThis action cannot be undone.", 
                        "Delete Task", 
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Warning);

                    if (confirmation == MessageBoxResult.Yes)
                    {
                        _taskService.RemoveTask(taskId);
                        MessageBox.Show("Task deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgTasks.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    // Get TaskId from dynamic object
                    int taskId = (int)selectedItem.GetType().GetProperty("TaskId").GetValue(selectedItem);
                    var updateStatusWindow = new UpdateTaskStatusWindow(taskId);
                    updateStatusWindow.ShowDialog();
                    LoadData(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating task status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a task to update status.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void dgTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = dgTasks.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    string taskName = (string)selectedItem.GetType().GetProperty("TaskName").GetValue(selectedItem);
                    string description = (string)selectedItem.GetType().GetProperty("Description").GetValue(selectedItem);
                    string status = (string)selectedItem.GetType().GetProperty("Status").GetValue(selectedItem);
                    DateTime deadline = (DateTime)selectedItem.GetType().GetProperty("Deadline").GetValue(selectedItem);
                    string assignedMembers = (string)selectedItem.GetType().GetProperty("AssignedMembers").GetValue(selectedItem);

                    MessageBox.Show($"Task Details:\n\nName: {taskName}\nDescription: {description}\nStatus: {status}\nDeadline: {deadline:dd/MM/yyyy}\nAssigned To: {assignedMembers}", 
                        "Task Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing task details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
