using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Model
{
    public class ProjectTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
