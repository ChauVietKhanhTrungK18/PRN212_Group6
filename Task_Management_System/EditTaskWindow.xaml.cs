using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TMS_BLL.IService;
using TMS_DAL.Model;
using Task_Management_System.Models;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly int _taskId;
        private int _projectId;
        private ProjectTask _task;
        private Project _project;
        private List<SelectableUser> _availableMembers;

        public EditTaskWindow(int taskId)
        {
            InitializeComponent();
            _taskId = taskId;
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
                _task = _taskService.GetById(_taskId);
                if (_task == null)
                {
                    MessageBox.Show("Task not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                _projectId = _task.ProjectId;

                _project = _projectService.GetById(_projectId);
                if (_project != null)
                {
                    txtProjectInfo.Text = $"Project: {_project.ProjectName}";
                }

                txtTaskName.Text = _task.TaskName;
                txtDescription.Text = _task.Description;
                dpDeadline.SelectedDate = _task.Deadline;

                foreach (ComboBoxItem item in cbStatus.Items)
                {
                    if (item.Content.ToString() == _task.Status)
                    {
                        cbStatus.SelectedItem = item;
                        break;
                    }
                }

                LoadMembersAndAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMembersAndAssignments()
        {
            try
            {
                var projectMembers = _projectMemberService.GetMembersByProject(_projectId);
                _availableMembers = projectMembers.Select(pm => new SelectableUser(pm.User)).ToList();

                var assignedMembers = _taskAssignmentService.GetAssignedMembers(_taskId);
                var assignedMemberIds = assignedMembers.Select(m => m.UserId).ToList();

                foreach (var member in _availableMembers)
                {
                    member.IsSelected = assignedMemberIds.Contains(member.User.UserId);
                }

                lbMembers.ItemsSource = _availableMembers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
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

            var selectedMembers = _availableMembers.Where(m => m.IsSelected).ToList();
            if (!selectedMembers.Any())
            {
                MessageBox.Show("Please select at least one member to assign the task.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _task.TaskName = taskName;
                _task.Description = description;
                _task.Deadline = (DateTime)deadline;
                _task.Status = status;

                _taskService.Update(_task);

                // Update task assignments - simply assign the selected members
                var memberIds = selectedMembers.Select(m => m.User.UserId).ToList();
                if (memberIds.Any())
                {
                    _taskAssignmentService.AssignTaskToMembers(_taskId, memberIds);
                }

                MessageBox.Show("Task updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 