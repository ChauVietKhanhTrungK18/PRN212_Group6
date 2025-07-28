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

        public IEnumerable<Notification> GetAll()
        {
            return _context.Notifications.ToList();
        }

        public Notification GetById(int id)
        {
            return _context.Notifications.Find(id);
        }

        public void Add(Notification notification)
        {
            _context.Notifications.Add(notification);
        }

        public void Delete(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null)
                _context.Notifications.Remove(notification);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Notification> GetByUserId(int userId)
        {
            return _context.Notifications.Where(n => n.UserId == userId).ToList();
        }

        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }
    }
} 