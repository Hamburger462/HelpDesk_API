using static System.Net.WebRequestMethods;

namespace HelpDesk.Configuration
{
    public static class AddCors
    {
        public static IServiceCollection AddClientCors(this IServiceCollection services, IConfiguration configuration) {
            services.AddCors(options =>
                options.AddDefaultPolicy(policy => policy.WithOrigins(configuration.GetConnectionString("AppUrl") ?? "http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials())
            );

            return services;
        }
    }
}
