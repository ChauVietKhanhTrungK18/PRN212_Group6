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
    /// Interaction logic for ManageMembersWindow.xaml
    /// </summary>
    public partial class ManageMembersWindow : Window
    {
        private readonly IProjectMemberService _projectMemberService;
        private readonly int _currentManagerId;

        public ManageMembersWindow(int managerId)
        {
            InitializeComponent();
            _currentManagerId = managerId;
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();

            LoadProjects();
        }

        private void LoadProjects()
        {
            cbProjects.ItemsSource = _projectMemberService.GetProjectsForManager(_currentManagerId); 
        }

        private void CbProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbProjects.SelectedItem is Project selectedProject)
            {
                dgMembers.ItemsSource = _projectMemberService.GetProjectMembersForProject(selectedProject.ProjectId);
            }
        }

        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {
            if (cbProjects.SelectedItem is Project selectedProject)
            {
                var addMemberWindow = new AddMemberWindow(selectedProject.ProjectId);
                addMemberWindow.ShowDialog();
            }
        }

        private void BtnRemoveMember_Click(object sender, RoutedEventArgs e)
        {
            if (dgMembers.SelectedItem is User selectedUser && cbProjects.SelectedItem is Project selectedProject)
            {
                _projectMemberService.RemoveMemberFromProject(selectedProject.ProjectId, selectedUser.UserId);
                MessageBox.Show($"Member '{selectedUser.FullName}' removed from project.");
            }
        }
    }

}
