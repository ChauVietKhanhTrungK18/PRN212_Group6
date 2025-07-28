using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TMS_BLL.IService;
using TMS_DAL.Model;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for NotificationsWindow.xaml
    /// </summary>
    public partial class NotificationsWindow : Window
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly int _currentUserId;
        private List<Notification> _allNotifications;

        public NotificationsWindow(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _notificationService = App.ServiceProvider.GetRequiredService<INotificationService>();
            _userService = App.ServiceProvider.GetRequiredService<IUserService>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load user information
                var user = _userService.GetById(_currentUserId);
                if (user != null)
                {
                    txtUserInfo.Text = $"User: {user.FullName}";
                }

                // Load notifications
                _allNotifications = _notificationService.GetNotificationsByUserId(_currentUserId).ToList();

                // Apply filters
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredNotifications = _allNotifications.AsQueryable();

                // Apply search filter
                string searchText = txtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    filteredNotifications = filteredNotifications.Where(n => 
                        n.Message.ToLower().Contains(searchText));
                }

                // Apply read status filter
                if (cbFilter.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                {
                    string selectedFilter = selectedItem.Content.ToString();
                    switch (selectedFilter)
                    {
                        case "Unread Only":
                            filteredNotifications = filteredNotifications.Where(n => !n.IsRead);
                            break;
                        case "Read Only":
                            filteredNotifications = filteredNotifications.Where(n => n.IsRead);
                            break;
                    }
                }

                // Apply sorting
                if (cbSortBy.SelectedItem is System.Windows.Controls.ComboBoxItem sortItem)
                {
                    string sortBy = sortItem.Content.ToString();
                    switch (sortBy)
                    {
                        case "Date (Oldest)":
                            filteredNotifications = filteredNotifications.OrderBy(n => n.DateCreated);
                            break;
                        case "Unread First":
                            filteredNotifications = filteredNotifications.OrderByDescending(n => n.IsRead).ThenByDescending(n => n.DateCreated);
                            break;
                        case "Date (Newest)":
                        default:
                            filteredNotifications = filteredNotifications.OrderByDescending(n => n.DateCreated);
                            break;
                    }
                }

                dgNotifications.ItemsSource = filteredNotifications.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cbSortBy_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnMarkAsRead_Click(object sender, RoutedEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notification selectedNotification)
            {
                try
                {
                    if (!selectedNotification.IsRead)
                    {
                        _notificationService.MarkAsRead(selectedNotification.NotificationId);
                        MessageBox.Show("Notification marked as read!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(); // Reload data
                    }
                    else
                    {
                        MessageBox.Show("This notification is already marked as read.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error marking notification as read: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a notification to mark as read.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnMarkAllAsRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var unreadNotifications = _allNotifications.Where(n => !n.IsRead).ToList();
                if (unreadNotifications.Any())
                {
                    var confirmation = MessageBox.Show(
                        $"Are you sure you want to mark all {unreadNotifications.Count} unread notifications as read?", 
                        "Mark All as Read", 
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Question);

                    if (confirmation == MessageBoxResult.Yes)
                    {
                        foreach (var notification in unreadNotifications)
                        {
                            _notificationService.MarkAsRead(notification.NotificationId);
                        }

                        MessageBox.Show($"All {unreadNotifications.Count} notifications marked as read!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(); // Reload data
                    }
                }
                else
                {
                    MessageBox.Show("No unread notifications to mark.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking all notifications as read: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notification selectedNotification)
            {
                var confirmation = MessageBox.Show(
                    $"Are you sure you want to delete this notification?", 
                    "Delete Notification", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (confirmation == MessageBoxResult.Yes)
                {
                    try
                    {
                        _notificationService.Delete(selectedNotification.NotificationId);
                        MessageBox.Show("Notification deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(); // Reload data
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting notification: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a notification to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void dgNotifications_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgNotifications.SelectedItem is Notification selectedNotification)
            {
                try
                {
                    // Mark as read when double-clicked
                    if (!selectedNotification.IsRead)
                    {
                        _notificationService.MarkAsRead(selectedNotification.NotificationId);
                    }

                    // Show notification details
                    MessageBox.Show($"Notification Details:\n\n{selectedNotification.Message}", 
                        "Notification Details", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    LoadData(); // Reload data
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error viewing notification details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 