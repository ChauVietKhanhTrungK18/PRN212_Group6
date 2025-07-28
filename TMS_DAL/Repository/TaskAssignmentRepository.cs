using System.Collections.Generic;
using System.Linq;
using TMS_DAL.Model;
using TMS_DAL.IRepository;
using TMS_DAL.Data;

namespace TMS_DAL.Repository
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public TaskAssignment GetById(int assignmentId) => _context.TaskAssignments.Find(assignmentId);
        
        public IEnumerable<TaskAssignment> GetByTaskId(int taskId) => 
            _context.TaskAssignments.Where(ta => ta.TaskId == taskId).ToList();
        
        public IEnumerable<TaskAssignment> GetByMemberId(int memberId) => 
            _context.TaskAssignments.Where(ta => ta.UserId == memberId).ToList();
        
        public void Add(TaskAssignment assignment)
        {
            _context.TaskAssignments.Add(assignment);
            _context.SaveChanges();
        }
        
        public void Remove(TaskAssignment assignment)
        {
            _context.TaskAssignments.Remove(assignment);
            _context.SaveChanges();
        }
        
        public void RemoveByTaskAndMember(int taskId, int memberId)
        {
            var assignment = _context.TaskAssignments
                .FirstOrDefault(ta => ta.TaskId == taskId && ta.UserId == memberId);
            if (assignment != null)
            {
                _context.TaskAssignments.Remove(assignment);
                _context.SaveChanges();
            }
        }
        public IEnumerable<TaskAssignment> GetAll() => _context.TaskAssignments.ToList();
        public void RemoveAllAssignmentsForTask(int taskId)
        {
            var assignments = _context.TaskAssignments
                .Where(ta => ta.TaskId == taskId)
                .ToList();

            if (assignments.Any())
            {
                _context.TaskAssignments.RemoveRange(assignments);
                _context.SaveChanges();
            }
        }
    }
} 