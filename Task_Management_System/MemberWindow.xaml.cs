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

            // Initialize collections
            _myTasks = new List<ProjectTask>();
            _myProjects = new List<Project>();
            _myNotifications = new List<Notification>();
            _myAttachments = new List<AttachmentViewModel>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _currentUser = _userService.GetById(_currentUserId);
                if (_currentUser != null)
                {
                    txtWelcomeMessage.Text = $"Welcome, {_currentUser.FullName}!";
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadMyTasks();
                    LoadMyProjects();
                    LoadMyNotifications();
                    //LoadMyAttachments();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
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
                var assignedTasks = _taskAssignmentService.GetAssignedTasks(_currentUserId);
                if (assignedTasks != null)
                {
                    var tasks = assignedTasks.ToList();
                    
                    // Auto-update task status to Overdue if past deadline and not completed
                    var currentDate = DateTime.Today;
                    var updatedTasks = new List<ProjectTask>();
                    
                    foreach (var task in tasks)
                    {
                        if (task.Status != "Completed" && currentDate > task.Deadline)
                        {
                            // Update task status to Overdue
                            task.Status = "Overdue";
                            try
                            {
                                _taskService.Update(task);
                            }
                            catch (Exception updateEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error updating task status to Overdue: {updateEx.Message}");
                            }
                        }
                        updatedTasks.Add(task);
                    }
                    
                    _myTasks = updatedTasks;
                    
                    // Apply filters to display data
                    ApplyTaskFilters();
                }
                else
                {
                    _myTasks = new List<ProjectTask>();
                    ApplyTaskFilters();
                }

                // Update task overview
                UpdateTaskOverview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _myTasks = new List<ProjectTask>();
                ApplyTaskFilters();
                UpdateTaskOverview();
            }
        }

        private void UpdateTaskOverview()
        {
            txtTotalTasks.Text = _myTasks.Count.ToString();
            txtInProgressTasks.Text = _myTasks.Count(t => t.Status == "In Progress").ToString();
            txtCompletedTasks.Text = _myTasks.Count(t => t.Status == "Completed").ToString();
            txtOverdueTasks.Text = _myTasks.Count(t => t.Status == "Overdue").ToString();
        }

        private void ApplyTaskFilters()
        {
            try
            {
                if (dgMyTasks == null || _myTasks == null)
                {
                    return;
                }

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

                // Create anonymous objects with ProjectName for display
                var currentDate = DateTime.Today;
                var tasksWithProject = filteredTasks.Select(task =>
                {
                    var project = _projectService.GetById(task.ProjectId);
                    var canMarkCompleted = task.Status == "In Progress" && currentDate <= task.Deadline;
                    
                    // Debug logging
                    System.Diagnostics.Debug.WriteLine($"Task: {task.TaskName}, Status: {task.Status}, Deadline: {task.Deadline}, CanMarkCompleted: {canMarkCompleted}");
                    
                    return new
                    {
                        TaskId = task.TaskId,
                        TaskName = task.TaskName,
                        Description = task.Description,
                        Deadline = task.Deadline,
                        Status = task.Status,
                        DateCreated = task.DateCreated,
                        ProjectId = task.ProjectId,
                        ProjectName = project?.ProjectName ?? "Unknown Project",
                        CanMarkCompleted = canMarkCompleted // Only show button if In Progress and not overdue
                    };
                }).ToList();

                dgMyTasks.ItemsSource = tasksWithProject;
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
            if (dgMyTasks.SelectedItem != null)
            {
                try
                {
                    var selectedTask = dgMyTasks.SelectedItem;
                    var taskId = selectedTask.GetType().GetProperty("TaskId")?.GetValue(selectedTask);
                    
                    if (taskId != null)
                    {
                        var updateStatusWindow = new UpdateTaskStatusWindow((int)taskId);
                        updateStatusWindow.ShowDialog();
                        LoadMyTasks(); // Reload data after status update
                    }
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
            if (dgMyTasks.SelectedItem != null)
            {
                try
                {
                    var selectedTask = dgMyTasks.SelectedItem;
                    var taskName = selectedTask.GetType().GetProperty("TaskName")?.GetValue(selectedTask)?.ToString();
                    var projectName = selectedTask.GetType().GetProperty("ProjectName")?.GetValue(selectedTask)?.ToString();
                    var description = selectedTask.GetType().GetProperty("Description")?.GetValue(selectedTask)?.ToString();
                    var status = selectedTask.GetType().GetProperty("Status")?.GetValue(selectedTask)?.ToString();
                    var deadline = selectedTask.GetType().GetProperty("Deadline")?.GetValue(selectedTask);

                    MessageBox.Show($"Task Details:\n\nName: {taskName}\nProject: {projectName}\nDescription: {description}\nStatus: {status}\nDeadline: {deadline:dd/MM/yyyy}", 
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

        private void BtnMarkTaskCompleted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var dataContext = button?.DataContext;
                
                if (dataContext != null)
                {
                    var taskId = dataContext.GetType().GetProperty("TaskId")?.GetValue(dataContext);
                    var deadline = dataContext.GetType().GetProperty("Deadline")?.GetValue(dataContext);
                    var status = dataContext.GetType().GetProperty("Status")?.GetValue(dataContext)?.ToString();
                    
                    if (taskId != null && deadline != null)
                    {
                        var currentDate = DateTime.Today;
                        var taskDeadline = (DateTime)deadline;
                        
                        // Check if task is overdue
                        if (currentDate > taskDeadline)
                        {
                            MessageBox.Show("Cannot mark task as completed because it is overdue!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        
                        // Check if task status is In Progress
                        if (status != "In Progress")
                        {
                            MessageBox.Show("Only tasks with 'In Progress' status can be marked as completed!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        
                        var confirmation = MessageBox.Show("Are you sure you want to mark this task as completed?", 
                            "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        
                        if (confirmation == MessageBoxResult.Yes)
                        {
                            // Update task status to completed
                            var task = _taskService.GetById((int)taskId);
                            if (task != null)
                            {
                                task.Status = "Completed";
                                _taskService.Update(task);
                                
                                MessageBox.Show("Task marked as completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                
                                // Refresh the task list
                                LoadMyTasks();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking task as completed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Projects Management

        private void LoadMyProjects()
        {
            try
            {
                // Get projects where current user is a member
                var projectMembers = _projectMemberService.GetProjectsByUserId(_currentUserId);
                if (projectMembers != null && projectMembers.Any())
                {
                    var projectIds = projectMembers.Select(pm => pm.ProjectId).ToList();
                    var projects = _projectService.GetAll().Where(p => projectIds.Contains(p.ProjectId)).ToList();
                    _myProjects = projects;
                    // Apply filters to display data
                    ApplyProjectFilters();
                }
                else
                {
                    _myProjects = new List<Project>();
                    ApplyProjectFilters();
                }

                // Update project overview
                UpdateProjectOverview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _myProjects = new List<Project>();
                ApplyProjectFilters();
                UpdateProjectOverview();
            }
        }

        private void UpdateProjectOverview()
        {
            txtTotalProjects.Text = _myProjects.Count.ToString();
            txtActiveProjects.Text = _myProjects.Count(p => p.Status == "In Progress").ToString();
            txtCompletedProjects.Text = _myProjects.Count(p => p.Status == "Completed").ToString();
        }

        private void ApplyProjectFilters()
        {
            try
            {
                if (dgMyProjects == null || _myProjects == null)
                {
                    return;
                }

                var filteredProjects = _myProjects.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrEmpty(txtSearchProjects?.Text))
                {
                    string searchTerm = txtSearchProjects.Text.ToLower();
                    filteredProjects = filteredProjects.Where(p => 
                        p.ProjectName.ToLower().Contains(searchTerm) || 
                        p.Description.ToLower().Contains(searchTerm));
                }

                // Create anonymous objects with ManagerName for display
                var projectsWithManager = filteredProjects.Select(project =>
                {
                    var manager = _userService.GetById(project.ManagerId);
                    return new
                    {
                        ProjectId = project.ProjectId,
                        ProjectName = project.ProjectName,
                        Description = project.Description,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                        Status = project.Status,
                        DateCreated = project.DateCreated,
                        ManagerId = project.ManagerId,
                        ManagerName = manager?.FullName ?? "Unknown"
                    };
                }).ToList();

                dgMyProjects.ItemsSource = projectsWithManager;
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
            if (dgMyProjects.SelectedItem != null)
            {
                try
                {
                    var selectedProject = dgMyProjects.SelectedItem;
                    var projectName = selectedProject.GetType().GetProperty("ProjectName")?.GetValue(selectedProject)?.ToString();
                    var description = selectedProject.GetType().GetProperty("Description")?.GetValue(selectedProject)?.ToString();
                    var status = selectedProject.GetType().GetProperty("Status")?.GetValue(selectedProject)?.ToString();
                    var startDate = selectedProject.GetType().GetProperty("StartDate")?.GetValue(selectedProject);
                    var endDate = selectedProject.GetType().GetProperty("EndDate")?.GetValue(selectedProject);
                    var managerName = selectedProject.GetType().GetProperty("ManagerName")?.GetValue(selectedProject)?.ToString();

                    MessageBox.Show($"Project Details:\n\nName: {projectName}\nDescription: {description}\nStatus: {status}\nStart Date: {startDate:dd/MM/yyyy}\nEnd Date: {endDate:dd/MM/yyyy}\nManager: {managerName}", 
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
            try
            {
                var button = sender as Button;
                var dataContext = button?.DataContext;
                
                if (dataContext != null)
                {
                    var projectId = dataContext.GetType().GetProperty("ProjectId")?.GetValue(dataContext);
                    
                    if (projectId != null)
                    {
                        var viewProjectTasksWindow = new ViewProjectTasksWindow((int)projectId);
                        viewProjectTasksWindow.ShowDialog();
                    }
                }
                else
                {
                    // Fallback to selected item if button context is null
                    if (dgMyProjects.SelectedItem != null)
                    {
                        var selectedProject = dgMyProjects.SelectedItem;
                        var projectId = selectedProject.GetType().GetProperty("ProjectId")?.GetValue(selectedProject);
                        
                        if (projectId != null)
                        {
                            var viewProjectTasksWindow = new ViewProjectTasksWindow((int)projectId);
                            viewProjectTasksWindow.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a project to view tasks.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing project tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var notifications = _notificationService.GetNotificationsByUserId(_currentUserId);
                if (notifications != null)
                {
                    _myNotifications = notifications.ToList();
                }
                else
                {
                    _myNotifications = new List<Notification>();
                }
                ApplyNotificationFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _myNotifications = new List<Notification>();
                ApplyNotificationFilters();
            }
        }

        private void ApplyNotificationFilters()
        {
            try
            {
                if (dgNotifications == null || _myNotifications == null)
                {
                    return;
                }

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

        //private void LoadMyAttachments()
        //{
        //    try
        //    {
        //        // Get attachments uploaded by current user
        //        var allAttachments = _attachmentService.GetAll();
        //        if (allAttachments != null)
        //        {
        //            var attachments = allAttachments.Where(a => a.UploadedByUserId == _currentUserId).ToList();
        //            _myAttachments = attachments.Select(a => new AttachmentViewModel(a)).ToList();
        //        }
        //        else
        //        {
        //            _myAttachments = new List<AttachmentViewModel>();
        //        }

        //        ApplyAttachmentFilters();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading attachments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        _myAttachments = new List<AttachmentViewModel>();
        //        ApplyAttachmentFilters();
        //    }
        //}

        //private void ApplyAttachmentFilters()
        //{
        //    try
        //    {
        //        if (dgAttachments == null || _myAttachments == null)
        //        {
        //            return;
        //        }

        //        var filteredAttachments = _myAttachments.AsEnumerable();

        //        // Apply search filter
        //        if (!string.IsNullOrEmpty(txtSearchAttachments.Text))
        //        {
        //            string searchTerm = txtSearchAttachments.Text.ToLower();
        //            filteredAttachments = filteredAttachments.Where(a => 
        //                a.FileName.ToLower().Contains(searchTerm));
        //        }

        //        dgAttachments.ItemsSource = filteredAttachments.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error applying attachment filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void txtSearchAttachments_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
        //    ApplyAttachmentFilters();
        //}

        //private void BtnRefreshAttachments_Click(object sender, RoutedEventArgs e)
        //{
        //    LoadMyAttachments();
        //}

        //private void BtnDownloadAttachment_Click(object sender, RoutedEventArgs e)
        //{
        //    if (dgAttachments.SelectedItem is AttachmentViewModel selectedAttachment)
        //    {
        //        try
        //        {
        //            MessageBox.Show($"Download functionality for '{selectedAttachment.FileName}' will be implemented.", 
        //                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error downloading attachment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select an attachment to download.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}

        //private void BtnViewAttachmentDetails_Click(object sender, RoutedEventArgs e)
        //{
        //    if (dgAttachments.SelectedItem is AttachmentViewModel selectedAttachment)
        //    {
        //        try
        //        {
        //            MessageBox.Show($"Attachment Details:\n\nFile Name: {selectedAttachment.FileName}\nRelated To: {selectedAttachment.RelatedTo}\nUpload Date: {selectedAttachment.DateUploaded:dd/MM/yyyy}\nFile Size: {selectedAttachment.FileSize}\nUploaded By: {selectedAttachment.UploadedBy}", 
        //                "Attachment Details", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error viewing attachment details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select an attachment to view details.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}

        //private void dgAttachments_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    BtnViewAttachmentDetails_Click(sender, e);
        //}

        #endregion

        #region Profile and Navigation

        private void BtnMyProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var updateProfileWindow = new UpdateProfileWindow(_currentUser);
                updateProfileWindow.ShowDialog();
                
                LoadData();
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
