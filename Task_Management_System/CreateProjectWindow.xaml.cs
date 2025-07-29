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
            
            // Initialize status preview
            UpdateStatusPreview();
        }

        private void BtnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            string projectName = txtProjectName.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;
            DateTime currentDate = DateTime.Today;

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

            if (startDate < currentDate)
            {
                MessageBox.Show("Start date must be greater than or equal to current date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpStartDate.Focus();
                return;
            }

            if (startDate >= endDate)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpEndDate.Focus();
                return;
            }

            string status = DetermineProjectStatus((DateTime)startDate, (DateTime)endDate, currentDate);

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
                MessageBox.Show($"Project created successfully with status: {status}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string DetermineProjectStatus(DateTime startDate, DateTime endDate, DateTime currentDate)
        {
            if (currentDate < startDate)
            {
                return "Not Started";
            }
            else if (currentDate >= startDate && currentDate <= endDate)
            {
                return "In Progress";
            }
            else
            {
                return "Completed";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateDateSelection();
        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateDateSelection();
        }

        private void ValidateDateSelection()
        {
            DateTime currentDate = DateTime.Today;
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            // Validate start date
            if (startDate.HasValue && startDate < currentDate)
            {
                MessageBox.Show("Start date cannot be in the past. Please select today or a future date.", 
                    "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpStartDate.SelectedDate = currentDate;
                return;
            }

            // Validate end date
            if (startDate.HasValue && endDate.HasValue && endDate <= startDate)
            {
                MessageBox.Show("End date must be after start date.", 
                    "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpEndDate.SelectedDate = startDate.Value.AddDays(1);
                return;
            }

            // Auto-update end date if it's not set or invalid
            if (startDate.HasValue && (!endDate.HasValue || endDate <= startDate))
            {
                dpEndDate.SelectedDate = startDate.Value.AddDays(1);
            }

            // Update status preview
            UpdateStatusPreview();
        }

        private void UpdateStatusPreview()
        {
            DateTime currentDate = DateTime.Today;
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            if (startDate.HasValue && endDate.HasValue)
            {
                string status = DetermineProjectStatus(startDate.Value, endDate.Value, currentDate);
                txtStatusPreview.Text = status;
                
                // Update color based on status
                switch (status)
                {
                    case "Not Started":
                        txtStatusPreview.Foreground = System.Windows.Media.Brushes.Orange;
                        break;
                    case "In Progress":
                        txtStatusPreview.Foreground = System.Windows.Media.Brushes.Green;
                        break;
                    case "Completed":
                        txtStatusPreview.Foreground = System.Windows.Media.Brushes.Blue;
                        break;
                    default:
                        txtStatusPreview.Foreground = System.Windows.Media.Brushes.Gray;
                        break;
                }
            }
            else
            {
                txtStatusPreview.Text = "Please select dates";
                txtStatusPreview.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
