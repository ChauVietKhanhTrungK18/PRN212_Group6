using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project GetById(int projectId) => _context.Projects.Find(projectId);
        public IEnumerable<Project> GetAll() => _context.Projects.ToList();
        public void Add(Project project) { _context.Projects.Add(project); _context.SaveChanges(); }
        public void Update(Project project) { _context.Projects.Update(project); _context.SaveChanges(); }
        public void Delete(int projectId)
        {
            var project = _context.Projects.Find(projectId);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
        }
    }
} 