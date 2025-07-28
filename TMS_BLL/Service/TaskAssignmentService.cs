using System.Collections.Generic;
using System.Linq;
using TMS_BLL.IService;
using TMS_DAL.IRepository;
using TMS_DAL.Model;

namespace TMS_BLL.Service
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public TaskAssignmentService(
            ITaskAssignmentRepository taskAssignmentRepository,
            IUserRepository userRepository,
            ITaskRepository taskRepository)
        {
            _taskAssignmentRepository = taskAssignmentRepository;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        public void AssignTaskToMember(int taskId, int memberId)
        {
            var assignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = memberId,
                AssignedAt = System.DateTime.Now,
            };
            _taskAssignmentRepository.Add(assignment);
        }

        public void AssignTaskToMembers(int taskId, List<int> memberIds)
        {
            foreach (var memberId in memberIds)
            {
                AssignTaskToMember(taskId, memberId);
            }
        }

        public void RemoveTaskAssignment(int taskId, int memberId)
        {
            _taskAssignmentRepository.RemoveByTaskAndMember(taskId, memberId);
        }

        public IEnumerable<TaskAssignment> GetTaskAssignments(int taskId)
        {
            return _taskAssignmentRepository.GetByTaskId(taskId);
        }

        public IEnumerable<TaskAssignment> GetMemberAssignments(int memberId)
        {
            return _taskAssignmentRepository.GetByMemberId(memberId);
        }

        public IEnumerable<User> GetAssignedMembers(int taskId)
        {
            var assignments = GetTaskAssignments(taskId);
            var memberIds = assignments.Select(a => a.UserId).ToList();
            return _userRepository.GetAll().Where(u => memberIds.Contains(u.UserId));
        }

        public IEnumerable<ProjectTask> GetAssignedTasks(int memberId)
        {
            var assignments = GetMemberAssignments(memberId);
            var taskIds = assignments.Select(a => a.TaskId).ToList();
            return _taskRepository.GetAll().Where(t => taskIds.Contains(t.TaskId));
        }
        public void RemoveAllAssignmentsForTask(int taskId)
        {
            _taskAssignmentRepository.RemoveAllAssignmentsForTask(taskId);
        }
    }
} 