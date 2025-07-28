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
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
