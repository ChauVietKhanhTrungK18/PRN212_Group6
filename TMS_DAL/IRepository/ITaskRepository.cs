using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface ITaskRepository
    {
        ProjectTask GetById(int taskId);
        IEnumerable<ProjectTask> GetByProjectId(int projectId);
        IEnumerable<ProjectTask> GetByAssignedUserId(int userId);
        void Add(ProjectTask task);
        void Update(ProjectTask task);
        void Delete(int taskId);
    }
} 