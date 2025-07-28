using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Model
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public int ManagerId { get; set; }
        public User? Manager { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
