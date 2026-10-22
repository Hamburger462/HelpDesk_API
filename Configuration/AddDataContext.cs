using HelpDesk.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Configuration
{
    public static class AddDataContext
    {
        public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")) // Add Connection string to the user-secrets
            );

            return services;
        }
    }
}