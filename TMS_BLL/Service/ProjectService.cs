using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IAttachmentRepository _attachmentRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IProjectMemberRepository projectMemberRepository,
            IAttachmentRepository attachmentRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _projectMemberRepository = projectMemberRepository;
            _attachmentRepository = attachmentRepository;
        }

        public Project GetById(int projectId) => _projectRepository.GetById(projectId);
        
        public IEnumerable<Project> GetAll() => _projectRepository.GetAll();
        
        public void Add(Project project) => _projectRepository.Add(project);
        
        public void Update(Project project) => _projectRepository.Update(project);
        
        public void Delete(int projectId)
        {
            // Get all related data
            var project = _projectRepository.GetById(projectId);
            if (project == null) return;

            // Delete related tasks
            var tasks = _taskRepository.GetAll().Where(t => t.ProjectId == projectId).ToList();
            foreach (var task in tasks)
            {
                _taskRepository.Delete(task.TaskId);
            }

            // Delete related project members
            var members = _projectMemberRepository.GetAll().Where(pm => pm.ProjectId == projectId).ToList();
            foreach (var member in members)
            {
                _projectMemberRepository.Delete(member.ProjectMemberId);
            }

            // Delete related attachments
            var attachments = _attachmentRepository.GetAll().Where(a => a.ProjectId == projectId).ToList();
            foreach (var attachment in attachments)
            {
                _attachmentRepository.Delete(attachment.AttachmentId);
            }

            // Finally delete the project
            _projectRepository.Delete(projectId);
        }
        
        public IEnumerable<Project> GetProjectsByManager(int managerId)
        {
            return _projectRepository.GetAll()
                                     .Where(p => p.ManagerId == managerId && !p.IsDeleted)
                                     .OrderByDescending(p => p.DateCreated)
                                     .ToList();
        }
    }
} 