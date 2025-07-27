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

        // Role hệ thống
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Các dự án mà người này tham gia
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<TaskAssignment> TaskAssignments { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
