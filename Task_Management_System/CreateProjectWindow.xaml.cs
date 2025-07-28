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
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        private readonly IProjectService _projectService;
        private readonly int _managerId;

        public CreateProjectWindow(int managerId)
        {
            InitializeComponent();
            _managerId = managerId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
        }

        private void BtnCreateProject_Click(object sender, RoutedEventArgs e)
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
                ProjectName = projectName,
                Description = description,
                StartDate = (DateTime)startDate,
                EndDate = (DateTime)endDate,
                ManagerId = _managerId,
                Status = "In Progress"  
            };

            _projectService.Add(project);
            MessageBox.Show("Project created successfully.");
            this.Close();
        }
    }

}
