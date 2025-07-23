using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_BLL.IService
{
    public interface IProjectService
    {
        Project GetById(int projectId);
        IEnumerable<Project> GetAll();
        void Add(Project project);
        void Update(Project project);
        void Delete(int projectId);
    }
} 