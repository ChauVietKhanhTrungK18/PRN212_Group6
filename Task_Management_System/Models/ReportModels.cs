using System;

namespace Task_Management_System.Models
{
    public class TaskStatusReport
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class MemberPerformanceReport
    {
        public string MemberName { get; set; }
        public int TotalAssigned { get; set; }
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int NotStarted { get; set; }
        public string CompletionRate { get; set; }
    }

    public class TaskDetailReport
    {
        public string TaskName { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedDate { get; set; }
    }
} 