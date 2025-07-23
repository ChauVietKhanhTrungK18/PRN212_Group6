using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Notification GetById(int notificationId) => _context.Notifications.Find(notificationId);
        public IEnumerable<Notification> GetByUserId(int userId) => _context.Notifications.Where(n => n.UserId == userId).ToList();
        public void Add(Notification notification) { _context.Notifications.Add(notification); _context.SaveChanges(); }
        public void MarkAsRead(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }
        public void Delete(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
        }
    }
} 