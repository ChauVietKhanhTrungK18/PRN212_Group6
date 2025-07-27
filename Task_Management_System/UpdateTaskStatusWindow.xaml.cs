using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class UpdateTaskStatusWindow : Window
    {
        private readonly ITaskService _taskService;
        private ProjectTask _task;

        public UpdateTaskStatusWindow(ProjectTask task)
        {
            InitializeComponent();
            _task = task;
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();
            LoadStatus();
        }

        private void LoadStatus()
        {
            for (int i = 0; i < cboStatus.Items.Count; i++)
            {
                var item = cboStatus.Items[i] as ComboBoxItem;
                if (item?.Content.ToString() == _task.Status)
                {
                    cboStatus.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            txtValidation.Text = "";
            if (cboStatus.SelectedItem == null)
            {
                txtValidation.Text = "Please select a status.";
                return;
            }
            var status = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Not Started";
            _task.Status = status;
            _taskService.Update(_task);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 