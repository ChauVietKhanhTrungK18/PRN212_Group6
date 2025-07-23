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
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public int AssignedTo { get; set; }
        public User AssignedUser { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
