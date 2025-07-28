using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Model;

namespace TMS_DAL.IRepository
{
    public interface IProjectMemberRepository
    {
        IEnumerable<User> GetMembersByProjectId(int projectId);
        void AddMemberToProject(int projectId, int userId);
        void RemoveMemberFromProject(int projectId, int userId);
        IEnumerable<Project> GetProjectsByManager(int managerId);
        IEnumerable<User> GetMembersByProjectIds(List<int> projectIds);
        IEnumerable<User> GetUsersNotInProject(int projectId);

    }
}
