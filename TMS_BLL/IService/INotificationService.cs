using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface INotificationService
    {
        Notification GetById(int notificationId);
        IEnumerable<Notification> GetByUserId(int userId);
        void Add(Notification notification);
        void MarkAsRead(int notificationId);
        void Delete(int notificationId);
    }
} 