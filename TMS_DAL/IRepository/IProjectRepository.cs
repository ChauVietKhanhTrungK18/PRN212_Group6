using TMS_DAL.Model;
using System.Collections.Generic;

namespace TMS_DAL.IRepository
{
    public interface IProjectRepository
    {
        Project GetById(int projectId);
        IEnumerable<Project> GetAll();
        void Add(Project project);
        void Update(Project project);
        void Delete(int projectId);
    }
} 