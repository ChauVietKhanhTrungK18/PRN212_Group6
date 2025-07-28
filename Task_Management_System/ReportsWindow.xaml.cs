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
    /// Interaction logic for ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly IUserService _userService;
        private readonly ITaskAssignmentService _taskAssignmentService;
        private readonly int _currentUserId;
        private List<Project> _userProjects;

        public ReportsWindow(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _taskAssignmentService = App.ServiceProvider.GetRequiredService<ITaskAssignmentService>();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load user info
                var currentUser = _userService.GetById(_currentUserId);
                if (currentUser != null)
                {
                    txtUserName.Text = currentUser.FullName;
                }

                // Load user's projects
                _userProjects = _projectService.GetProjectsByManager(_currentUserId).ToList();
                cbProjects.ItemsSource = _userProjects;
                cbProjects.DisplayMemberPath = "ProjectName";
                cbProjects.SelectedIndex = 0; // Select first project by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbProjects_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbProjects.SelectedItem is Project selectedProject)
            {
                txtProjectName.Text = $"Project: {selectedProject.ProjectName}";
                GenerateReport(selectedProject.ProjectId);
            }
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (cbProjects.SelectedItem is Project selectedProject)
            {
                GenerateReport(selectedProject.ProjectId);
            }
            else
            {
                MessageBox.Show("Please select a project to generate report.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GenerateReport(int projectId)
        {
            try
            {
                // Get project tasks
                var tasks = _taskService.GetTasksForProject(projectId).ToList();

                // Get project members
                var projectMembers = _projectMemberService.GetMembersByProject(projectId);
                var members = projectMembers.Select(pm => pm.User).ToList();

                // Update all report sections
                UpdateProjectOverview(tasks);
                UpdateTaskStatusDistribution(tasks);
                UpdateMemberPerformance(tasks, members);
                UpdateTaskDetails(tasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateProjectOverview(IEnumerable<ProjectTask> tasks)
        {
            int totalTasks = tasks.Count();
            int completedTasks = tasks.Count(t => t.Status == "Completed");
            int inProgressTasks = tasks.Count(t => t.Status == "In Progress");
            int notStartedTasks = tasks.Count(t => t.Status == "Not Started");

            txtTotalTasks.Text = totalTasks.ToString();
            txtCompletedTasks.Text = completedTasks.ToString();
            txtInProgressTasks.Text = inProgressTasks.ToString();
            txtNotStartedTasks.Text = notStartedTasks.ToString();

            // Update progress bar
            double completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks : 0;
            pbCompletion.Value = completionRate * 100;
            txtCompletionRate.Text = $"{completionRate * 100:F1}%";
        }

        private void UpdateTaskStatusDistribution(IEnumerable<ProjectTask> tasks)
        {
            var statusGroups = tasks.GroupBy(t => t.Status)
                .Select(g => new TaskStatusReport
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = tasks.Count() > 0 ? (double)g.Count() / tasks.Count() * 100 : 0
                }).ToList();

            dgStatusDistribution.ItemsSource = statusGroups;
        }

        private void UpdateMemberPerformance(IEnumerable<ProjectTask> tasks, IEnumerable<User> members)
        {
            var memberPerformance = new List<MemberPerformanceReport>();

            foreach (var member in members)
            {
                // Get tasks assigned to this member
                var assignedTasks = _taskAssignmentService.GetAssignedTasks(member.UserId);

                var performance = new MemberPerformanceReport
                {
                    MemberName = member.FullName,
                    TotalAssigned = assignedTasks.Count(),
                    Completed = assignedTasks.Count(t => t.Status == "Completed"),
                    InProgress = assignedTasks.Count(t => t.Status == "In Progress"),
                    NotStarted = assignedTasks.Count(t => t.Status == "Not Started"),
                    CompletionRate = assignedTasks.Any() 
                        ? $"{(double)assignedTasks.Count(t => t.Status == "Completed") / assignedTasks.Count() * 100:F1}%" 
                        : "0%"
                };

                memberPerformance.Add(performance);
            }

            dgMemberPerformance.ItemsSource = memberPerformance;

            // Update performance summary
            if (memberPerformance.Any())
            {
                var topPerformer = memberPerformance.OrderByDescending(m => m.Completed).First();
                txtTopPerformer.Text = topPerformer.MemberName;

                double avgCompletionRate = memberPerformance.Average(m => 
                {
                    if (double.TryParse(m.CompletionRate.TrimEnd('%'), out double rate))
                        return rate;
                    return 0;
                });
                txtAvgCompletionRate.Text = $"{avgCompletionRate:F1}%";

                txtTotalMembers.Text = memberPerformance.Count.ToString();
            }
        }

        private void UpdateTaskDetails(IEnumerable<ProjectTask> tasks)
        {
            var taskDetails = tasks.Select(t => new TaskDetailReport
            {
                TaskName = t.TaskName,
                Status = t.Status,
                AssignedTo = GetAssignedMembersString(t.TaskId),
                Deadline = t.Deadline,
                CreatedDate = t.DateCreated
            }).ToList();

            dgTaskDetails.ItemsSource = taskDetails;
        }

        private string GetAssignedMembersString(int taskId)
        {
            try
            {
                var assignedMembers = _taskAssignmentService.GetAssignedMembers(taskId);
                return string.Join(", ", assignedMembers.Select(m => m.FullName));
            }
            catch
            {
                return "N/A";
            }
        }

        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Implement CSV export
                MessageBox.Show("CSV export feature will be implemented in the next phase.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Implement PDF export
                MessageBox.Show("PDF export feature will be implemented in the next phase.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Implement print functionality
                MessageBox.Show("Print feature will be implemented in the next phase.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 