using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Model
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateUploaded { get; set; }
        public int UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; }
        public int? TaskId { get; set; }
        public ProjectTask Task { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
