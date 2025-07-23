using System.Collections.Generic;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class ProjectRoleService : IProjectRoleService
    {
        private readonly IProjectRoleRepository _projectRoleRepository;
        public ProjectRoleService(IProjectRoleRepository projectRoleRepository)
        {
            _projectRoleRepository = projectRoleRepository;
        }

        public ProjectRole GetById(int projectRoleId) => _projectRoleRepository.GetById(projectRoleId);
        public IEnumerable<ProjectRole> GetByProjectId(int projectId) => _projectRoleRepository.GetByProjectId(projectId);
        public IEnumerable<ProjectRole> GetByUserId(int userId) => _projectRoleRepository.GetByUserId(userId);
        public void Add(ProjectRole projectRole) => _projectRoleRepository.Add(projectRole);
        public void Update(ProjectRole projectRole) => _projectRoleRepository.Update(projectRole);
        public void Delete(int projectRoleId) => _projectRoleRepository.Delete(projectRoleId);
        public void RemoveUserFromProject(int userId, int projectId) => _projectRoleRepository.RemoveUserFromProject(userId, projectId);
    }
} 