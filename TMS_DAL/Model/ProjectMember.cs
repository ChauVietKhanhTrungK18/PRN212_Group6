using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Model
{
    public class ProjectMember
    {
        public int ProjectMemberId { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public Project? Project { get; set; }
        public User? User { get; set; }
    }
}
