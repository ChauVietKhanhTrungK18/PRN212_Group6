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
            
            // Set default dates
            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today.AddMonths(1);
        }

        private void BtnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            string projectName = txtProjectName.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;
            string status = cbStatus.Text;

            // Validation
            if (string.IsNullOrEmpty(projectName))
            {
                MessageBox.Show("Please enter project name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProjectName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Please enter project description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return;
            }

            if (startDate == null)
            {
                MessageBox.Show("Please select start date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpStartDate.Focus();
                return;
            }

            if (endDate == null)
            {
                MessageBox.Show("Please select end date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpEndDate.Focus();
                return;
            }

            if (startDate >= endDate)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpEndDate.Focus();
                return;
            }

            try
            {
                var project = new Project
                {
                    ProjectName = projectName,
                    Description = description,
                    StartDate = (DateTime)startDate,
                    EndDate = (DateTime)endDate,
                    Status = status,
                    ManagerId = _managerId,
                    DateCreated = DateTime.Now
                };

                _projectService.Add(project);
                MessageBox.Show("Project created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
