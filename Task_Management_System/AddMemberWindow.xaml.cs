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
    /// Interaction logic for AddMemberWindow.xaml
    /// </summary>
    public partial class AddMemberWindow : Window
    {
        private readonly IProjectMemberService _projectMemberService;
        private readonly IUserService _userService;
        private readonly int _projectId;

        public AddMemberWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;  
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();

            LoadUsers();  
        }

        private void LoadUsers()
        {
            try
            {
                var usersNotInProject = _projectMemberService.GetUsersNotInProject(_projectId);
                var availableMembers = usersNotInProject.Where(u => u.RoleId == 3 && !u.IsDeleted).ToList();
                
                cbUsers.ItemsSource = availableMembers;
                cbUsers.DisplayMemberPath = "FullName";
                
                if (availableMembers.Count == 0)
                {
                    MessageBox.Show("No available members to add. All members are already in this project or there are no regular members available.", 
                        "No Available Members", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {
            if (cbUsers.SelectedItem is User selectedUser)
            {
                try
                {
                    _projectMemberService.AddMemberToProject(_projectId, selectedUser.UserId);
                    MessageBox.Show($"Member '{selectedUser.FullName}' added to the project successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();  
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a member to add.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
