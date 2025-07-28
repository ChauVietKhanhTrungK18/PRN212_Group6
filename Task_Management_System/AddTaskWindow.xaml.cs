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
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly int _projectId;

        public AddTaskWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string taskName = txtTaskName.Text.Trim();
            string description = txtDescription.Text.Trim();
            DateTime? deadline = dpDeadline.SelectedDate;

            if (string.IsNullOrEmpty(taskName) || deadline == null)
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            var newTask = new ProjectTask
            {
                ProjectId = _projectId,
                TaskName = taskName,
                Description = description,
                Deadline = (DateTime)deadline,
                Status = "Not Started"
            };

            _taskService.AddTask(newTask);
            MessageBox.Show("Task added successfully.");
            this.Close();
        }
    }

}
