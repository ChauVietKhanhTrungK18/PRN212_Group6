using System.Collections.Generic;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public IEnumerable<Notification> GetAll()
        {
            return _notificationRepository.GetAll();
        }

        public Notification GetById(int id)
        {
            return _notificationRepository.GetById(id);
        }

        public void Add(Notification notification)
        {
            _notificationRepository.Add(notification);
            _notificationRepository.SaveChanges();
        }

        public void Delete(int id)
        {
            _notificationRepository.Delete(id);
            _notificationRepository.SaveChanges();
        }

        public IEnumerable<Notification> GetNotificationsByUserId(int userId)
        {
            return _notificationRepository.GetByUserId(userId);
        }

        public void MarkAsRead(int notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _notificationRepository.Update(notification);
                _notificationRepository.SaveChanges();
            }
        }

        public void MarkAllAsRead(int userId)
        {
            var notifications = _notificationRepository.GetByUserId(userId);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _notificationRepository.Update(notification);
            }
            _notificationRepository.SaveChanges();
        }
    }
} 