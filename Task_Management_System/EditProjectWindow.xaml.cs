using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for EditProjectWindow.xaml
    /// </summary>
    public partial class EditProjectWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly int _projectId;

        public EditProjectWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadProjectDetails();
        }

        private void LoadProjectDetails()
        {
            var project = _projectService.GetById(_projectId);
            if (project != null)
            {
                txtProjectName.Text = project.ProjectName;
                txtDescription.Text = project.Description;
                dpStartDate.SelectedDate = project.StartDate;
                dpEndDate.SelectedDate = project.EndDate;
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string projectName = txtProjectName.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            if (string.IsNullOrEmpty(projectName) || startDate == null || endDate == null)
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            var project = new Project
            {
                ProjectId = _projectId,
                ProjectName = projectName,
                Description = description,
                StartDate = (DateTime)startDate,
                EndDate = (DateTime)endDate,
                Status = "In Progress" 
            };

            _projectService.Update(project);
            MessageBox.Show("Project updated successfully.");
            this.Close();
        }
    }

}
