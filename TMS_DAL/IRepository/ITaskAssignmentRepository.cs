using System.Collections.Generic;
using TMS_DAL.Model;

namespace TMS_DAL.IRepository
{
    public interface ITaskAssignmentRepository
    {
        TaskAssignment GetById(int assignmentId);
        IEnumerable<TaskAssignment> GetByTaskId(int taskId);
        IEnumerable<TaskAssignment> GetByMemberId(int memberId);
        void Add(TaskAssignment assignment);
        void Remove(TaskAssignment assignment);
        void RemoveByTaskAndMember(int taskId, int memberId);
        IEnumerable<TaskAssignment> GetAll();
        void RemoveAllAssignmentsForTask(int taskId);
    }
} 