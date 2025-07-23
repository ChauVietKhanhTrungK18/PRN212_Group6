using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TMS_DAL.Data;
using TMS_DAL.Repository;
using TMS_BLL.Service;
using TMS_DAL.IRepository;

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
            services.AddScoped<UserService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<RoleService>();
            services.AddScoped<TaskService>();
            services.AddScoped<AttachmentService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<ProjectRoleService>();
            services.AddScoped<ReportService>();
        }
    }
}
