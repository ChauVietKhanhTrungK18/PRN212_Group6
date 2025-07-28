using System;
using System.IO;
using TMS_DAL.Model;

namespace Task_Management_System.Models
{
    public class AttachmentViewModel
    {
        public string FileName { get; set; } = string.Empty;
        public string RelatedTo { get; set; } = string.Empty;
        public DateTime DateUploaded { get; set; }
        public string FileSize { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;

        public AttachmentViewModel()
        {
        }

        public AttachmentViewModel(Attachment attachment)
        {
            Attachment = attachment;
            FileName = attachment?.FileName ?? string.Empty;
            RelatedTo = attachment?.ProjectId.HasValue == true ? "Project" : "Task";
            DateUploaded = attachment?.DateUploaded ?? DateTime.Now;
            FileSize = GetFileSize(attachment?.FilePath);
            UploadedBy = attachment?.UploadedByUser?.FullName ?? string.Empty;
        }

        public Attachment Attachment { get; set; }

        private string GetFileSize(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    return FormatFileSize(fileInfo.Length);
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
} 