using System.Collections.Generic;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        public AttachmentService(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public Attachment GetById(int attachmentId) => _attachmentRepository.GetById(attachmentId);
        public IEnumerable<Attachment> GetByTaskId(int taskId) => _attachmentRepository.GetByTaskId(taskId);
        public IEnumerable<Attachment> GetByProjectId(int projectId) => _attachmentRepository.GetByProjectId(projectId);
        public void Add(Attachment attachment) => _attachmentRepository.Add(attachment);
        public void Delete(int attachmentId) => _attachmentRepository.Delete(attachmentId);
    }
} 