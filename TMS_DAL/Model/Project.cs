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
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }

        public int ManagerId { get; set; }
        public User Manager { get; set; }

        public ICollection<ProjectRole> ProjectRoles { get; set; }
        public ICollection<ProjectTask> ProjectTasks { get; set; }
    }
}
