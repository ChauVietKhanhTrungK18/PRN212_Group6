using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TMS_BLL.IService;
using TMS_BLL.Service;
using TMS_DAL.Data;
using TMS_DAL.IRepository;
using TMS_DAL.Repository;

namespace Task_Management_System
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();
            // Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IProjectRoleRepository, ProjectRoleRepository>();
            // Service

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IProjectRoleService, ProjectRoleService>();
            services.AddScoped<IReportService, ReportService>();
        }
    }
}
