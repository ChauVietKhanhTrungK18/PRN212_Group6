using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<ProjectRole> ProjectRoles { get; set; }
        public ICollection<ProjectTask> ProjectTasks { get; set; }
    }
}
