using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Application.Services;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IReportService, ReportService>();
        }
    }
}
