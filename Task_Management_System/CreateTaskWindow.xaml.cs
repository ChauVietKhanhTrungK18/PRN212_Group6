using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TMS_BLL.IService;
using TMS_DAL.Model;
using Task_Management_System.Models;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly int _projectId;
        private Project _project;
        private List<SelectableUser> _availableMembers;

        public CreateTaskWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();
            _taskAssignmentService = App.ServiceProvider.GetRequiredService<ITaskAssignmentService>();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load project info
                _project = _projectService.GetById(_projectId);
                if (_project != null)
                {
                    txtProjectInfo.Text = _project.ProjectName;
                }

                // Load project members
                var projectMembers = _projectMemberService.GetMembersByProject(_projectId);
                _availableMembers = projectMembers.Select(pm => new SelectableUser(pm.User)).ToList();

                lbMembers.ItemsSource = _availableMembers;

                // Set default deadline to tomorrow
                dpDeadline.SelectedDate = DateTime.Today.AddDays(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var member in _availableMembers)
            {
                member.IsSelected = true;
            }
            lbMembers.Items.Refresh();
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var member in _availableMembers)
            {
                member.IsSelected = false;
            }
            lbMembers.Items.Refresh();
        }

        private void BtnCreateTask_Click(object sender, RoutedEventArgs e)
        {
            string taskName = txtTaskName.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime? deadline = dpDeadline.SelectedDate;
            string status = cbStatus.Text;

            // Validation
            if (string.IsNullOrEmpty(taskName))
            {
                MessageBox.Show("Please enter task name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTaskName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Please enter task description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return;
            }

            if (deadline == null)
            {
                MessageBox.Show("Please select deadline.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDeadline.Focus();
                return;
            }

            if (deadline < DateTime.Today)
            {
                MessageBox.Show("Deadline cannot be in the past.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDeadline.Focus();
                return;
            }

            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbStatus.Focus();
                return;
            }

            var selectedMembers = _availableMembers.Where(m => m.IsSelected).ToList();
            if (!selectedMembers.Any())
            {
                MessageBox.Show("Please select at least one member to assign the task.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Debug: Check if project exists
                var projectCheck = _projectService.GetById(_projectId);
                if (projectCheck == null)
                {
                    MessageBox.Show($"Project with ID {_projectId} not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var task = new ProjectTask
                {
                    TaskName = taskName,
                    Description = description,
                    Deadline = (DateTime)deadline,
                    Status = status,
                    ProjectId = _projectId,
                    DateCreated = DateTime.Now
                };

                // Debug: Log task data
                System.Diagnostics.Debug.WriteLine($"Creating task: Name={taskName}, ProjectId={_projectId}, Status={status}");

                int taskId = _taskService.AddTask(task);

                if (taskId > 0)
                {
                    // Assign task to selected members
                    try
                    {
                        var memberIds = selectedMembers.Select(m => m.User.UserId).ToList();
                        _taskAssignmentService.AssignTaskToMembers(taskId, memberIds);
                        MessageBox.Show($"Task '{taskName}' created successfully and assigned to {selectedMembers.Count} member(s)!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception assignmentEx)
                    {
                        // If there's a database column error in assignment, still show success but warn about assignment
                        System.Diagnostics.Debug.WriteLine($"Error assigning task to members: {assignmentEx.Message}");
                        MessageBox.Show($"Task '{taskName}' created successfully, but there was an issue assigning members to the task.\n\nError: {assignmentEx.Message}", 
                            "Task Created with Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create task. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error creating task: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 