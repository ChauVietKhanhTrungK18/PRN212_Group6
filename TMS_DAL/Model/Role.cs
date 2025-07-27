using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Model
{
    public class Role
    {
        public int RoleId { get; set; } // Admin, Manager, Member
        public string RoleName { get; set; }
        public string Description { get; set; }

        // Quan hệ
        public ICollection<User> Users { get; set; }
    }
}
