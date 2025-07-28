using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TMS_BLL.IService;
using TMS_DAL.Model;
using Task_Management_System.Models;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for ManageAttachmentsWindow.xaml
    /// </summary>
    public partial class ManageAttachmentsWindow : Window
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly int _currentUserId;
        private readonly int? _projectId;
        private readonly int? _taskId;
        private List<AttachmentViewModel> _attachments;
        private List<string> _selectedFiles;

        public ManageAttachmentsWindow(int currentUserId, int? projectId = null, int? taskId = null)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _projectId = projectId;
            _taskId = taskId;
            _attachmentService = App.ServiceProvider.GetRequiredService<IAttachmentService>();
            _projectService = App.ServiceProvider.GetRequiredService<IProjectService>();
            _taskService = App.ServiceProvider.GetRequiredService<ITaskService>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (_projectId.HasValue)
                {
                    var project = _projectService.GetById(_projectId.Value);
                    txtTitle.Text = "Project Attachments";
                    txtInfo.Text = $"Project: {project?.ProjectName}";
                    LoadProjectAttachments();
                }
                else if (_taskId.HasValue)
                {
                    var task = _taskService.GetById(_taskId.Value);
                    txtTitle.Text = "Task Attachments";
                    txtInfo.Text = $"Task: {task?.TaskName}";
                    LoadTaskAttachments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProjectAttachments()
        {
            var attachments = _attachmentService.GetByProjectId(_projectId.Value);
            _attachments = attachments.Select(a => new AttachmentViewModel(a)).ToList();
            ApplyFilters();
        }

        private void LoadTaskAttachments()
        {
            var attachments = _attachmentService.GetByTaskId(_taskId.Value);
            _attachments = attachments.Select(a => new AttachmentViewModel(a)).ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                var filteredAttachments = _attachments.AsQueryable();

                if (!string.IsNullOrEmpty(searchText))
                {
                    filteredAttachments = filteredAttachments.Where(a => 
                        a.FileName.ToLower().Contains(searchText) || 
                        a.UploadedBy.ToLower().Contains(searchText));
                }

                dgAttachments.ItemsSource = filteredAttachments.ToList();
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

        private void BtnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select Files to Upload"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFiles = openFileDialog.FileNames.ToList();
                txtSelectedFiles.Text = $"{_selectedFiles.Count} file(s) selected";
            }
        }

        private void BtnUploadFiles_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFiles == null || !_selectedFiles.Any())
            {
                MessageBox.Show("Please select files to upload.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                int uploadedCount = 0;
                foreach (var filePath in _selectedFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    var destinationPath = GetUploadPath(fileName);

                    // Copy file to upload directory
                    File.Copy(filePath, destinationPath, true);

                    // Create attachment record
                    var attachment = new Attachment
                    {
                        FileName = fileName,
                        FilePath = destinationPath,
                        DateUploaded = DateTime.Now,
                        UploadedByUserId = _currentUserId,
                        ProjectId = _projectId,
                        TaskId = _taskId
                    };

                    _attachmentService.Add(attachment);
                    uploadedCount++;
                }

                MessageBox.Show($"{uploadedCount} file(s) uploaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _selectedFiles = null;
                txtSelectedFiles.Text = "No files selected";
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetUploadPath(string fileName)
        {
            var uploadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var uniqueFileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}";
            return Path.Combine(uploadDir, uniqueFileName);
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttachments.SelectedItem is AttachmentViewModel selectedAttachment)
            {
                try
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        FileName = selectedAttachment.FileName,
                        Title = "Save File"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.Copy(selectedAttachment.Attachment.FilePath, saveFileDialog.FileName, true);
                        MessageBox.Show("File downloaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a file to download.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttachments.SelectedItem is AttachmentViewModel selectedAttachment)
            {
                var confirmation = MessageBox.Show(
                    $"Are you sure you want to delete '{selectedAttachment.FileName}'?", 
                    "Delete File", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (confirmation == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Delete physical file
                        if (File.Exists(selectedAttachment.Attachment.FilePath))
                        {
                            File.Delete(selectedAttachment.Attachment.FilePath);
                        }

                        // Delete database record
                        _attachmentService.Delete(selectedAttachment.Attachment.AttachmentId);

                        MessageBox.Show("File deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a file to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 