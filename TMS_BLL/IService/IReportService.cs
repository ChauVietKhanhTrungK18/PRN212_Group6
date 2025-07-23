using System.Collections.Generic;
using TMS_DAL.Model;

namespace TMS_BLL.IService
{
    public interface IReportService
    {
        object GetProjectProgress(int projectId);
        object GetUserPerformance(int userId);
        object GetTaskStatistics(int projectId);
    }
} 