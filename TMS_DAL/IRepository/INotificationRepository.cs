using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface INotificationRepository
    {
        Notification GetById(int notificationId);
        IEnumerable<Notification> GetByUserId(int userId);
        void Add(Notification notification);
        void MarkAsRead(int notificationId);
        void Delete(int notificationId);
    }
} 