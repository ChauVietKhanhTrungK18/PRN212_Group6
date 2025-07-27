using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    public partial class ManageAccountsWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private List<User> allUsers;
        private List<Role> roles;

        public ManageAccountsWindow()
        {
            InitializeComponent();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();
            _roleService = App.ServiceProvider.GetRequiredService<IRoleService>();
            LoadRoles();
            LoadUsers();
        }

        private void LoadRoles()
        {
            roles = _roleService.GetAll().Where(r => r.RoleId != 1).ToList(); 
            cbRoleFilter.ItemsSource = roles;
        }

        private void LoadUsers()
        {
            allUsers = _userService.GetAll(true).Where(u => u.RoleId != 1).ToList(); 
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (dgUsers == null) return;            
            if (allUsers == null) allUsers = new List<User>();

            var filtered = allUsers;

            string searchText = txtSearch.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(u =>
                    (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(searchText))
                ).ToList();
            }

            if (cbRoleFilter.SelectedItem is Role selectedRole)
            {
                filtered = filtered.Where(u => u.RoleId == selectedRole.RoleId).ToList();
            }

            if (cbStatusFilter.SelectedItem is ComboBoxItem statusItem)
            {
                string status = statusItem.Content.ToString();
                if (status == "Active")
                    filtered = filtered.Where(u => !u.IsDeleted).ToList();
                else if (status == "Inactive")
                    filtered = filtered.Where(u => u.IsDeleted).ToList() ;
            }

            dgUsers.ItemsSource = filtered;

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbRoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cbRoleFilter.SelectedIndex = -1;
            cbStatusFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void dgUsers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is User user)
            {
                _userService.UpdateRole(user.UserId, user.RoleId);
                MessageBox.Show($"Role updated for {user.Username}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                if (!user.IsDeleted)
                {
                    MessageBox.Show("This user is already active.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                _userService.SetDeletedStatus(user.UserId, false);
                MessageBox.Show("User activated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
                ApplyFilters();
            }
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                if (user.IsDeleted)
                {
                    MessageBox.Show("This user is already inactive.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                _userService.SetDeletedStatus(user.UserId, true);
                MessageBox.Show("User deactivated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
                ApplyFilters();
            }
        }
        private int _selectedUserId;

        private void BtnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is not User user)
            {
                MessageBox.Show("Please select a user first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (user.RoleId == 1) 
            {
                MessageBox.Show("Cannot change role of Admin users.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedUserId = user.UserId;
            var roles = _roleService.GetAll().Where(r => r.RoleId != 1).ToList();
            cbPopupRoles.ItemsSource = roles;
            cbPopupRoles.SelectedValue = user.RoleId;

            PopupChangeRole.Visibility = Visibility.Visible;
        }
        private void BtnCancelPopup_Click(object sender, RoutedEventArgs e)
        {
            PopupChangeRole.Visibility = Visibility.Collapsed;
        }
        private void BtnSavePopup_Click(object sender, RoutedEventArgs e)
        {
            if (cbPopupRoles.SelectedValue is int newRoleId)
            {
                _userService.UpdateRole(_selectedUserId, newRoleId);
                PopupChangeRole.Visibility = Visibility.Collapsed;
                LoadUsers();
                ApplyFilters();
                MessageBox.Show("Role changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow();
            adminWindow.Show();
            this.Close();
        }


    }
} 