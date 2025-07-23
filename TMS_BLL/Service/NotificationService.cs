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

        public Notification GetById(int notificationId) => _notificationRepository.GetById(notificationId);
        public IEnumerable<Notification> GetByUserId(int userId) => _notificationRepository.GetByUserId(userId);
        public void Add(Notification notification) => _notificationRepository.Add(notification);
        public void MarkAsRead(int notificationId) => _notificationRepository.MarkAsRead(notificationId);
        public void Delete(int notificationId) => _notificationRepository.Delete(notificationId);
    }
} 