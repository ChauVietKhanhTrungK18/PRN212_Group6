using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Data;
using TMS_DAL.IRepository;
using TMS_DAL.Model;

namespace TMS_DAL.Repository
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetMembersByProjectId(int projectId)
        {
            return _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Select(pm => pm.User)
                .ToList();
        }

        public void AddMemberToProject(int projectId, int userId)
        {
            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId
            };
            _context.ProjectMembers.Add(projectMember);
            _context.SaveChanges();
        }

        public void RemoveMemberFromProject(int projectId, int userId)
        {
            var projectMember = _context.ProjectMembers
                .FirstOrDefault(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (projectMember != null)
            {
                _context.ProjectMembers.Remove(projectMember);
                _context.SaveChanges();
            }
        }
        public IEnumerable<Project> GetProjectsByManager(int managerId)
        {
            return _context.Projects
                           .Where(p => p.ManagerId == managerId) 
                           .ToList();
        }
        public IEnumerable<User> GetMembersByProjectIds(List<int> projectIds)
        {
            return _context.ProjectMembers
                           .Where(pm => projectIds.Contains(pm.ProjectId))
                           .Select(pm => pm.User)
                           .ToList();
        }
        public IEnumerable<User> GetUsersNotInProject(int projectId)
        {
            var usersInProject = _context.ProjectMembers
                                          .Where(pm => pm.ProjectId == projectId)
                                          .Select(pm => pm.UserId)
                                          .ToList();

            return _context.Users
                           .Where(u => !usersInProject.Contains(u.UserId))
                           .ToList();
        }
    }
}
