using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface INotificationRepository
    {
        IEnumerable<Notification> GetAll();
        Notification GetById(int id);
        void Add(Notification notification);
        void Delete(int id);
        void SaveChanges();
        IEnumerable<Notification> GetByUserId(int userId);
        void Update(Notification notification);
    }
} 