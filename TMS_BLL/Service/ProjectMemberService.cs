using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_BLL.IService;
using TMS_DAL.IRepository;
using TMS_DAL.Model;

namespace TMS_BLL.Service
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IProjectMemberRepository _projectMemberRepository;

        public ProjectMemberService(IProjectMemberRepository projectMemberRepository)
        {
            _projectMemberRepository = projectMemberRepository;
        }

        public IEnumerable<User> GetProjectMembers(int projectId)
        {
            return _projectMemberRepository.GetMembersByProjectId(projectId);
        }

        public void AddMemberToProject(int projectId, int userId)
        {
            _projectMemberRepository.AddMemberToProject(projectId, userId);
        }

        public void RemoveMemberFromProject(int projectId, int userId)
        {
            _projectMemberRepository.RemoveMemberFromProject(projectId, userId);
        }
        public IEnumerable<User> GetProjectMembersForManager(int managerId)
        {
            var projectIds = _projectMemberRepository.GetProjectsByManager(managerId)
                                                      .Select(p => p.ProjectId)
                                                      .ToList();

            return _projectMemberRepository.GetMembersByProjectIds(projectIds);
        }
        public IEnumerable<Project> GetProjectsForManager(int managerId)
        {
            return _projectMemberRepository.GetProjectsByManager(managerId);
        }

        public IEnumerable<User> GetProjectMembersForProject(int projectId)
        {
            return _projectMemberRepository.GetMembersByProjectId(projectId);
        }
        public IEnumerable<User> GetUsersNotInProject(int projectId)
        {
            return _projectMemberRepository.GetUsersNotInProject(projectId); 
        }

        public IEnumerable<ProjectMember> GetMembersByProject(int projectId)
        {
            return _projectMemberRepository.GetAll().Where(pm => pm.ProjectId == projectId);
        }

        public IEnumerable<ProjectMember> GetMembersByProjectId(int projectId)
        {
            return _projectMemberRepository.GetAll().Where(pm => pm.ProjectId == projectId);
        }
    }
}
