using System.Collections.Generic;
using TMS_DAL.Model;

namespace TMS_BLL.IService
{
    public interface ITaskAssignmentService
    {
        void AssignTaskToMember(int taskId, int memberId);
        void AssignTaskToMembers(int taskId, List<int> memberIds);
        void RemoveTaskAssignment(int taskId, int memberId);
        IEnumerable<TaskAssignment> GetTaskAssignments(int taskId);
        IEnumerable<TaskAssignment> GetMemberAssignments(int memberId);
        IEnumerable<User> GetAssignedMembers(int taskId);
        IEnumerable<ProjectTask> GetAssignedTasks(int memberId);
        void RemoveAllAssignmentsForTask(int taskId);
    }
} 