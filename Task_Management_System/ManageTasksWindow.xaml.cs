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
    /// Interaction logic for ManageTasksWindow.xaml
    /// </summary>
    public partial class ManageTasksWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly int _currentManagerId;

        public ManageTasksWindow(int managerId)
        {
            InitializeComponent();
            _currentManagerId = managerId;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            LoadProjects();
        }

        private void LoadProjects()
        {
            cbProjects.ItemsSource = _taskService.GetProjectsForManager(_currentManagerId);
        }

        private void CbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbProjects.SelectedItem is Project selectedProject)
            {
                dgTasks.ItemsSource = _taskService.GetTasksForProject(selectedProject.ProjectId);
            }
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            if (cbProjects.SelectedItem is Project selectedProject)
            {
                var addTaskWindow = new AddTaskWindow(selectedProject.ProjectId);
                addTaskWindow.ShowDialog();
            }
        }

        private void BtnRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is ProjectTask selectedTask)
            {
                _taskService.RemoveTask(selectedTask.TaskId);
                MessageBox.Show($"Task '{selectedTask.TaskName}' removed from project.");
                CbProjects_SelectionChanged(sender, new SelectionChangedEventArgs(ComboBox.SelectionChangedEvent, null, null));
            }
        }
    }

}
