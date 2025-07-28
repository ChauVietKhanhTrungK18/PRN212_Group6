using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Model;

namespace TMS_BLL.IService
{
    public interface IProjectMemberService
    {
        IEnumerable<User> GetProjectMembers(int projectId);
        void AddMemberToProject(int projectId, int userId);
        void RemoveMemberFromProject(int projectId, int userId);
        IEnumerable<User> GetProjectMembersForManager(int managerId);
        IEnumerable<Project> GetProjectsForManager(int managerId);
        IEnumerable<User> GetProjectMembersForProject(int projectId);
        IEnumerable<User> GetUsersNotInProject(int projectId);
    }
}
