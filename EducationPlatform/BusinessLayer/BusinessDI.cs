using BusinessLayer.Implementation;
using BusinessLayer.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer
{
    public static class BusinessDI
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BusinessDI).Assembly);

            services.AddScoped<IAcademicService, AcademicService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<ISpeechToTextService, SpeechToTextService>();
            services.AddScoped<IAIService, AIService>();

            return services;
        }
    }
}
