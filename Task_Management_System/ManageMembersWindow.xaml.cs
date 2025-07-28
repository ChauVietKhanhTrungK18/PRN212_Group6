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
        private readonly IProjectService _projectService;
        private readonly int _currentManagerId;
        private readonly int _selectedProjectId;
        private Project _selectedProject;

        public ManageMembersWindow(int managerId, int projectId)
        {
            InitializeComponent();
            _currentManagerId = managerId;
            _selectedProjectId = projectId;
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();

            LoadProjectInfo();
            LoadProjectMembers();
        }

        private void LoadProjectInfo()
        {
            try
            {
                _selectedProject = _projectService.GetById(_selectedProjectId);
                if (_selectedProject != null)
                {
                    txtProjectTitle.Text = $"Manage Members - {_selectedProject.ProjectName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProjectMembers()
        {
            try
            {
                // Get only the members (users) of this project, excluding the manager
                var projectMembers = _projectMemberService.GetProjectMembersForProject(_selectedProjectId);
                dgMembers.ItemsSource = projectMembers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addMemberWindow = new AddMemberWindow(_selectedProjectId);
                addMemberWindow.ShowDialog();
                
                LoadProjectMembers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening add member window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoveMember_Click(object sender, RoutedEventArgs e)
        {
            if (dgMembers.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove '{selectedUser.FullName}' from the project?", 
                    "Confirm Removal", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _projectMemberService.RemoveMemberFromProject(_selectedProjectId, selectedUser.UserId);
                        MessageBox.Show($"Member '{selectedUser.FullName}' removed from project successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Refresh the member list
                        LoadProjectMembers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error removing member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a member to remove.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProjectMembers();
        }
    }
}
