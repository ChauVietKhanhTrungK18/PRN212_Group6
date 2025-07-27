using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface INotificationService
    {
        IEnumerable<Notification> GetAll();
        Notification GetById(int id);
        void Add(Notification notification);
        void Delete(int id);
    }
} 