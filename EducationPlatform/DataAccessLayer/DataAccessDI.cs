using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer
{
    public static class DataAccessDI
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("Server");

            services.AddDbContext<EducationPlatformDBContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IAIImprovementSessionRepository, AIImprovementSessionRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPolicyRepository, PolicyRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
