using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface IProjectRoleService
    {
        ProjectRole GetById(int projectRoleId);
        IEnumerable<ProjectRole> GetByProjectId(int projectId);
        IEnumerable<ProjectRole> GetByUserId(int userId);
        void Add(ProjectRole projectRole);
        void Update(ProjectRole projectRole);
        void Delete(int projectRoleId);
        void RemoveUserFromProject(int userId, int projectId);
    }
} 