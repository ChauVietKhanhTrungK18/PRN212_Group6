using System.Collections.Generic;
using TMS_DAL.Model;

namespace TMS_BLL.IService
{
    public interface ITaskService
    {
        ProjectTask GetById(int taskId);
        IEnumerable<ProjectTask> GetByProjectId(int projectId);
        IEnumerable<ProjectTask> GetByAssignedUserId(int userId);
        int AddTask(ProjectTask task);
        void Update(ProjectTask task);
        void UpdateStatus(int taskId, string status);
        IEnumerable<Project> GetProjectsForManager(int managerId);
        IEnumerable<ProjectTask> GetTasksForProject(int projectId);
        void RemoveTask(int taskId);
        IEnumerable<ProjectTask> GetAll();
    }
} 