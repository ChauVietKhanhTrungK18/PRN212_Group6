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
    /// Interaction logic for MemberWindow.xaml
    /// </summary>
    public partial class MemberWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly INotificationService _notificationService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserService _userService;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly int _currentUserId;
        private User _currentUser;

        // Data collections
        private List<ProjectTask> _myTasks;
        private List<Project> _myProjects;
        private List<Notification> _myNotifications;
        private List<AttachmentViewModel> _myAttachments;

        public MemberWindow(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            
            // Initialize services
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();
            _notificationService = App.ServiceProvider.GetRequiredService<INotificationService>();
            _attachmentService = App.ServiceProvider.GetRequiredService<IAttachmentService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _taskAssignmentService = App.ServiceProvider.GetRequiredService<ITaskAssignmentService>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load current user
                _currentUser = _userService.GetById(_currentUserId);
                if (_currentUser != null)
                {
                    txtWelcomeMessage.Text = $"Welcome, {_currentUser.FullName}!";
                }

                // Load all data
                LoadMyTasks();
                LoadMyProjects();
                LoadMyNotifications();
                LoadMyAttachments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Tasks Management

        private void LoadMyTasks()
        {
            try
            {
                // Get tasks assigned to current user
                _myTasks = _taskAssignmentService.GetAssignedTasks(_currentUserId).ToList();

                // Update task overview
                UpdateTaskOverview();

                // Apply filters
                ApplyTaskFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTaskOverview()
        {
            if (_myTasks != null)
            {
                txtTotalTasks.Text = _myTasks.Count.ToString();
                txtInProgressTasks.Text = _myTasks.Count(t => t.Status == "In Progress").ToString();
                txtCompletedTasks.Text = _myTasks.Count(t => t.Status == "Completed").ToString();
                txtOverdueTasks.Text = _myTasks.Count(t => t.Deadline < DateTime.Today && t.Status != "Completed").ToString();
            }
        }

        private void ApplyTaskFilters()
        {
            try
            {
                var filteredTasks = _myTasks.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrEmpty(txtSearchTasks.Text))
                {
                    string searchTerm = txtSearchTasks.Text.ToLower();
                    filteredTasks = filteredTasks.Where(t => 
                        t.TaskName.ToLower().Contains(searchTerm) || 
                        t.Description.ToLower().Contains(searchTerm));
                }

                // Apply status filter
                if (cbTaskStatusFilter.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                {
                    string selectedStatus = selectedItem.Content.ToString();
                    if (selectedStatus != "All Tasks")
                    {
                        filteredTasks = filteredTasks.Where(t => t.Status == selectedStatus);
                    }
                }

                dgMyTasks.ItemsSource = filteredTasks.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying task filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchTasks_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyTaskFilters();
        }

        private void cbTaskStatusFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyTaskFilters();
        }

        private void BtnRefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            LoadMyTasks();
        }

        private void BtnUpdateTaskStatus_Click(object sender, RoutedEventArgs e)
        {
            if (dgMyTasks.SelectedItem is ProjectTask selectedTask)
            {
                try
                {
                    var updateStatusWindow = new UpdateTaskStatusWindow(selectedTask.TaskId);
                    updateStatusWindow.ShowDialog();
                    LoadMyTasks(); // Reload data after status update
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

        private void BtnViewTaskDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgMyTasks.SelectedItem is ProjectTask selectedTask)
            {
                try
                {
                    MessageBox.Show($"Task Details:\n\nName: {selectedTask.TaskName}\nDescription: {selectedTask.Description}\nStatus: {selectedTask.Status}\nDeadline: {selectedTask.Deadline:dd/MM/yyyy}", 
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

        private void dgMyTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnViewTaskDetails_Click(sender, e);
        }

        #endregion

        #region Projects Management

        private void LoadMyProjects()
        {
            try
            {
                // Get projects where current user is a member
                var projectMembers = _projectMemberService.GetMembersByProjectId(_currentUserId);
                var projectIds = projectMembers.Select(pm => pm.ProjectId).ToList();
                _myProjects = _projectService.GetAll().Where(p => projectIds.Contains(p.ProjectId)).ToList();

                // Update project overview
                UpdateProjectOverview();

                // Apply filters
                ApplyProjectFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateProjectOverview()
        {
            if (_myProjects != null)
            {
                txtTotalProjects.Text = _myProjects.Count.ToString();
                txtActiveProjects.Text = _myProjects.Count(p => p.Status == "Active").ToString();
                txtCompletedProjects.Text = _myProjects.Count(p => p.Status == "Completed").ToString();
            }
        }

        private void ApplyProjectFilters()
        {
            try
            {
                var filteredProjects = _myProjects.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrEmpty(txtSearchProjects.Text))
                {
                    string searchTerm = txtSearchProjects.Text.ToLower();
                    filteredProjects = filteredProjects.Where(p => 
                        p.ProjectName.ToLower().Contains(searchTerm) || 
                        p.Description.ToLower().Contains(searchTerm));
                }

                dgMyProjects.ItemsSource = filteredProjects.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying project filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchProjects_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyProjectFilters();
        }

        private void BtnRefreshProjects_Click(object sender, RoutedEventArgs e)
        {
            LoadMyProjects();
        }

        private void BtnViewProjectDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgMyProjects.SelectedItem is Project selectedProject)
            {
                try
                {
                    MessageBox.Show($"Project Details:\n\nName: {selectedProject.ProjectName}\nDescription: {selectedProject.Description}\nStatus: {selectedProject.Status}\nStart Date: {selectedProject.StartDate:dd/MM/yyyy}\nEnd Date: {selectedProject.EndDate:dd/MM/yyyy}", 
                        "Project Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing project details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a project to view details.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnViewProjectTasks_Click(object sender, RoutedEventArgs e)
        {
            if (dgMyProjects.SelectedItem is Project selectedProject)
            {
                try
                {
                    var manageTasksWindow = new ManageTasksWindow(selectedProject.ProjectId);
                    manageTasksWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing project tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a project to view tasks.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgMyProjects_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnViewProjectDetails_Click(sender, e);
        }

        #endregion

        #region Notifications Management

        private void LoadMyNotifications()
        {
            try
            {
                _myNotifications = _notificationService.GetNotificationsByUserId(_currentUserId).ToList();
                ApplyNotificationFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyNotificationFilters()
        {
            try
            {
                var filteredNotifications = _myNotifications.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrEmpty(txtSearchNotifications.Text))
                {
                    string searchTerm = txtSearchNotifications.Text.ToLower();
                    filteredNotifications = filteredNotifications.Where(n => 
                        n.Message.ToLower().Contains(searchTerm));
                }

                // Apply status filter
                if (cbNotificationFilter.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                {
                    string selectedFilter = selectedItem.Content.ToString();
                    switch (selectedFilter)
                    {
                        case "Unread Only":
                            filteredNotifications = filteredNotifications.Where(n => !n.IsRead);
                            break;
                        case "Read Only":
                            filteredNotifications = filteredNotifications.Where(n => n.IsRead);
                            break;
                    }
                }

                dgNotifications.ItemsSource = filteredNotifications.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying notification filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchNotifications_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyNotificationFilters();
        }

        private void cbNotificationFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyNotificationFilters();
        }

        private void BtnRefreshNotifications_Click(object sender, RoutedEventArgs e)
        {
            LoadMyNotifications();
        }

        private void BtnMarkAsRead_Click(object sender, RoutedEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notification selectedNotification)
            {
                try
                {
                    _notificationService.MarkAsRead(selectedNotification.NotificationId);
                    MessageBox.Show("Notification marked as read.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMyNotifications();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error marking notification as read: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a notification to mark as read.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnMarkAllAsRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirmation = MessageBox.Show("Are you sure you want to mark all notifications as read?", 
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (confirmation == MessageBoxResult.Yes)
                {
                    _notificationService.MarkAllAsRead(_currentUserId);
                    MessageBox.Show("All notifications marked as read.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMyNotifications();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking all notifications as read: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteNotification_Click(object sender, RoutedEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notification selectedNotification)
            {
                try
                {
                    var confirmation = MessageBox.Show("Are you sure you want to delete this notification?", 
                        "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (confirmation == MessageBoxResult.Yes)
                    {
                        _notificationService.Delete(selectedNotification.NotificationId);
                        MessageBox.Show("Notification deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMyNotifications();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting notification: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a notification to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgNotifications_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notification selectedNotification)
            {
                try
                {
                    _notificationService.MarkAsRead(selectedNotification.NotificationId);
                    MessageBox.Show($"Notification Details:\n\nMessage: {selectedNotification.Message}\nDate: {selectedNotification.DateCreated:dd/MM/yyyy HH:mm}", 
                        "Notification Details", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMyNotifications();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing notification details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Attachments Management

        private void LoadMyAttachments()
        {
            try
            {
                // Get attachments uploaded by current user
                var attachments = _attachmentService.GetAll().Where(a => a.UploadedByUserId == _currentUserId).ToList();
                _myAttachments = attachments.Select(a => new AttachmentViewModel(a)).ToList();

                ApplyAttachmentFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attachments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyAttachmentFilters()
        {
            try
            {
                var filteredAttachments = _myAttachments.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrEmpty(txtSearchAttachments.Text))
                {
                    string searchTerm = txtSearchAttachments.Text.ToLower();
                    filteredAttachments = filteredAttachments.Where(a => 
                        a.FileName.ToLower().Contains(searchTerm));
                }

                dgAttachments.ItemsSource = filteredAttachments.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying attachment filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchAttachments_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyAttachmentFilters();
        }

        private void BtnRefreshAttachments_Click(object sender, RoutedEventArgs e)
        {
            LoadMyAttachments();
        }

        private void BtnDownloadAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttachments.SelectedItem is AttachmentViewModel selectedAttachment)
            {
                try
                {
                    MessageBox.Show($"Download functionality for '{selectedAttachment.FileName}' will be implemented.", 
                        "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading attachment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an attachment to download.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnViewAttachmentDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttachments.SelectedItem is AttachmentViewModel selectedAttachment)
            {
                try
                {
                    MessageBox.Show($"Attachment Details:\n\nFile Name: {selectedAttachment.FileName}\nRelated To: {selectedAttachment.RelatedTo}\nUpload Date: {selectedAttachment.DateUploaded:dd/MM/yyyy}\nFile Size: {selectedAttachment.FileSize}\nUploaded By: {selectedAttachment.UploadedBy}", 
                        "Attachment Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing attachment details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an attachment to view details.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgAttachments_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnViewAttachmentDetails_Click(sender, e);
        }

        #endregion

        #region Profile and Navigation

        private void BtnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Profile management feature will be implemented in the next phase.", 
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changePasswordWindow = new ChangePasswordWindow(_currentUserId);
                changePasswordWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening change password window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var confirmation = MessageBox.Show("Are you sure you want to logout?", 
                    "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (confirmation == MessageBoxResult.Yes)
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
