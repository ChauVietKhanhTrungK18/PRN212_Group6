using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;
        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Attachment GetById(int attachmentId) => _context.Attachments.Find(attachmentId);
        public IEnumerable<Attachment> GetByTaskId(int taskId) => _context.Attachments.Where(a => a.TaskId == taskId).ToList();
        public IEnumerable<Attachment> GetByProjectId(int projectId) => _context.Attachments.Where(a => a.ProjectId == projectId).ToList();
        public void Add(Attachment attachment) { _context.Attachments.Add(attachment); _context.SaveChanges(); }
        public void Delete(int attachmentId)
        {
            var attachment = _context.Attachments.Find(attachmentId);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                _context.SaveChanges();
            }
        }
    }
} 