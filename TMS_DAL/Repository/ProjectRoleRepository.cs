using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class ProjectRoleRepository : IProjectRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProjectRole GetById(int projectRoleId) => _context.ProjectRoles.Find(projectRoleId);
        public IEnumerable<ProjectRole> GetByProjectId(int projectId) => _context.ProjectRoles.Where(pr => pr.ProjectId == projectId).ToList();
        public IEnumerable<ProjectRole> GetByUserId(int userId) => _context.ProjectRoles.Where(pr => pr.UserId == userId).ToList();
        public void Add(ProjectRole projectRole) { _context.ProjectRoles.Add(projectRole); _context.SaveChanges(); }
        public void Update(ProjectRole projectRole) { _context.ProjectRoles.Update(projectRole); _context.SaveChanges(); }
        public void Delete(int projectRoleId)
        {
            var pr = _context.ProjectRoles.Find(projectRoleId);
            if (pr != null)
            {
                _context.ProjectRoles.Remove(pr);
                _context.SaveChanges();
            }
        }
        public void RemoveUserFromProject(int userId, int projectId)
        {
            var pr = _context.ProjectRoles.FirstOrDefault(x => x.UserId == userId && x.ProjectId == projectId);
            if (pr != null)
            {
                _context.ProjectRoles.Remove(pr);
                _context.SaveChanges();
            }
        }
    }
} 