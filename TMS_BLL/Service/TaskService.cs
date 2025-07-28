using System.Collections.Generic;
using System.Linq;
using TMS_BLL.IService;
using TMS_DAL.IRepository;
using TMS_DAL.Model;
using TMS_DAL.Repository;

namespace TMS_BLL.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public ProjectTask GetById(int taskId) => _taskRepository.GetById(taskId);
        public IEnumerable<ProjectTask> GetByProjectId(int projectId) => _taskRepository.GetByProjectId(projectId);
        public IEnumerable<ProjectTask> GetByAssignedUserId(int userId) => _taskRepository.GetByAssignedUserId(userId);
        public int AddTask(ProjectTask task)
        {
            _taskRepository.Add(task);
            // Get the created task ID by finding the latest task for this project
            var createdTask = _taskRepository.GetByProjectId(task.ProjectId)
                .OrderByDescending(t => t.DateCreated)
                .FirstOrDefault();
            return createdTask?.TaskId ?? 0;
        }
        public void Update(ProjectTask task) => _taskRepository.Update(task);
        public void UpdateStatus(int taskId, string status)
        {
            var task = _taskRepository.GetById(taskId);
            if (task != null)
            {
                task.Status = status;
                _taskRepository.Update(task);
            }
        }
        public IEnumerable<Project> GetProjectsForManager(int managerId)
        {
            return _projectRepository.GetAll().Where(p => p.ManagerId == managerId).ToList();
        }

        public IEnumerable<ProjectTask> GetTasksForProject(int projectId)
        {
            return _taskRepository.GetAll().Where(t => t.ProjectId == projectId).ToList();
        }
        public void RemoveTask(int taskId)
        {
            var task = _taskRepository.GetById(taskId);
            if (task != null)
            {
                _taskRepository.Delete(task);
            }
        }
        public IEnumerable<ProjectTask> GetAll() => _taskRepository.GetAll();
    }
} 