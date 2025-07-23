using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface IAttachmentRepository
    {
        Attachment GetById(int attachmentId);
        IEnumerable<Attachment> GetByTaskId(int taskId);
        IEnumerable<Attachment> GetByProjectId(int projectId);
        void Add(Attachment attachment);
        void Delete(int attachmentId);
    }
} 