using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProjectTask GetById(int taskId) => _context.ProjectTasks.Find(taskId);
        public IEnumerable<ProjectTask> GetByProjectId(int projectId) => _context.ProjectTasks.Where(t => t.ProjectId == projectId).ToList();
        public IEnumerable<ProjectTask> GetByAssignedUserId(int userId) => _context.ProjectTasks.Where(t => t.AssignedTo == userId).ToList();
        public void Add(ProjectTask task) { _context.ProjectTasks.Add(task); _context.SaveChanges(); }
        public void Update(ProjectTask task) { _context.ProjectTasks.Update(task); _context.SaveChanges(); }
        public void Delete(int taskId)
        {
            var task = _context.ProjectTasks.Find(taskId);
            if (task != null)
            {
                _context.ProjectTasks.Remove(task);
                _context.SaveChanges();
            }
        }
    }
} 