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
        private Project _currentProject;

        public EditProjectWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            LoadProjectDetails();
        }

        private void LoadProjectDetails()
        {
            try
            {
                _currentProject = _projectService.GetById(_projectId);
                if (_currentProject != null)
                {
                    txtProjectName.Text = _currentProject.ProjectName;
                    txtDescription.Text = _currentProject.Description;
                    dpStartDate.SelectedDate = _currentProject.StartDate;
                    dpEndDate.SelectedDate = _currentProject.EndDate;
                    
                    // Set the correct status in ComboBox
                    foreach (ComboBoxItem item in cbStatus.Items)
                    {
                        if (item.Content.ToString() == _currentProject.Status)
                        {
                            cbStatus.SelectedItem = item;
                            break;
                        }
                    }
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

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
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
                    ProjectId = _projectId,
                    ProjectName = projectName,
                    Description = description,
                    StartDate = (DateTime)startDate,
                    EndDate = (DateTime)endDate,
                    Status = status,
                    ManagerId = _currentProject.ManagerId,
                    DateCreated = _currentProject.DateCreated
                };

                _projectService.Update(project);
                MessageBox.Show("Project updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
