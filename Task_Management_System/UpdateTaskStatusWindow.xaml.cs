using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for UpdateTaskStatusWindow.xaml
    /// </summary>
    public partial class UpdateTaskStatusWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly int _taskId;
        private ProjectTask _task;

        public UpdateTaskStatusWindow(int taskId)
        {
            InitializeComponent();
            _taskId = taskId;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();

            LoadTaskDetails();
        }

        private void LoadTaskDetails()
        {
            try
            {
                _task = _taskService.GetById(_taskId);
                if (_task != null)
                {
                    txtTaskInfo.Text = $"Task: {_task.TaskName}";
                    txtTaskName.Text = _task.TaskName;
                    txtDescription.Text = _task.Description;
                    txtDeadline.Text = _task.Deadline.ToString("dd/MM/yyyy");
                    txtCurrentStatus.Text = _task.Status;

                    // Set the current status as selected in ComboBox
                    foreach (System.Windows.Controls.ComboBoxItem item in cbNewStatus.Items)
                    {
                        if (item.Content.ToString() == _task.Status)
                        {
                            cbNewStatus.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Task not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading task details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            string newStatus = cbNewStatus.Text;

            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Please select a new status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newStatus == _task.Status)
            {
                MessageBox.Show("The new status is the same as the current status.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _taskService.UpdateStatus(_taskId, newStatus);
                MessageBox.Show($"Task status updated successfully from '{_task.Status}' to '{newStatus}'!", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 