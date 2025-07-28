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
        private readonly int _projectId;

        public AddMemberWindow(int projectId)
        {
            InitializeComponent();
            _projectId = projectId;  
            _projectMemberService = App.ServiceProvider.GetRequiredService<IProjectMemberService>();

            LoadUsers();  
        }

        private void LoadUsers()
        {
            var usersNotInProject = _projectMemberService.GetUsersNotInProject(_projectId);
            cbUsers.ItemsSource = usersNotInProject;  
        }

        private void BtnAddMember_Click(object sender, RoutedEventArgs e)
        {
            if (cbUsers.SelectedItem is User selectedUser)
            {
                _projectMemberService.AddMemberToProject(_projectId, selectedUser.UserId);
                MessageBox.Show($"Member '{selectedUser.FullName}' added to the project.");
                this.Close();  
            }
            else
            {
                MessageBox.Show("Please select a member to add.");
            }
        }
    }
}
