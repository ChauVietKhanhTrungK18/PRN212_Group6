using System.Collections.Generic;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public ProjectTask GetById(int taskId) => _taskRepository.GetById(taskId);
        public IEnumerable<ProjectTask> GetByProjectId(int projectId) => _taskRepository.GetByProjectId(projectId);
        public IEnumerable<ProjectTask> GetByAssignedUserId(int userId) => _taskRepository.GetByAssignedUserId(userId);
        public void Add(ProjectTask task) => _taskRepository.Add(task);
        public void Update(ProjectTask task) => _taskRepository.Update(task);
        public void Delete(int taskId) => _taskRepository.Delete(taskId);
        public void UpdateStatus(int taskId, string status)
        {
            var task = _taskRepository.GetById(taskId);
            if (task != null)
            {
                task.Status = status;
                _taskRepository.Update(task);
            }
        }
    }
} 