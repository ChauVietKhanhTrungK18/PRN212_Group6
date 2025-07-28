using TMS_DAL.Model;

namespace Task_Management_System.Models
{
    public class SelectableUser
    {
        public User User { get; set; }
        public bool IsSelected { get; set; }

        public string FullName => User?.FullName ?? "";
        public string Email => User?.Email ?? "";

        public SelectableUser(User user)
        {
            User = user;
            IsSelected = false;
        }
    }
} 