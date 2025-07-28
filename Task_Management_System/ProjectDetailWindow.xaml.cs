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
    /// Interaction logic for ProjectDetailWindow.xaml
    /// </summary>
    public partial class ProjectDetailWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly int _projectId;
        private Project _project;

        public ProjectDetailWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();

            LoadProjectDetails();
        }

        private void LoadProjectDetails()
        {
            try
            {
                _project = _projectService.GetById(_projectId);
                if (_project != null)
                {
                    // Set project information
                    txtProjectTitle.Text = _project.ProjectName;
                    txtProjectStatus.Text = $"Status: {_project.Status}";
                    txtProjectName.Text = _project.ProjectName;
                    txtProjectDescription.Text = _project.Description;
                    txtStartDate.Text = _project.StartDate.ToString("dd/MM/yyyy");
                    txtEndDate.Text = _project.EndDate.ToString("dd/MM/yyyy");
                    txtCreatedDate.Text = _project.DateCreated.ToString("dd/MM/yyyy");

                    // Load statistics
                    LoadProjectStatistics();
                }
                else
                {
                    MessageBox.Show("Project not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LoadProjectStatistics()
        {
            try
            {
                var tasks = _taskService.GetTasksForProject(_projectId);
                int totalTasks = tasks.Count();
                int completedTasks = tasks.Count(t => t.Status == "Completed");

                var members = _projectMemberService.GetMembersByProjectId(_projectId);
                int teamMembers = members.Count();

                txtTotalTasks.Text = totalTasks.ToString();
                txtCompletedTasks.Text = completedTasks.ToString();
                txtTeamMembers.Text = teamMembers.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 