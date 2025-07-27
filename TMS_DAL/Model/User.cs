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

        public int RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<TaskAssignment> TaskAssignments { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
