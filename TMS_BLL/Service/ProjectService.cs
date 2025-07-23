using System.Collections.Generic;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_BLL.IService;

namespace TMS_BLL.Service
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public Project GetById(int projectId) => _projectRepository.GetById(projectId);
        public IEnumerable<Project> GetAll() => _projectRepository.GetAll();
        public void Add(Project project) => _projectRepository.Add(project);
        public void Update(Project project) => _projectRepository.Update(project);
        public void Delete(int projectId) => _projectRepository.Delete(projectId);
    }
} 