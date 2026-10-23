using System.Threading.RateLimiting;

namespace HelpDesk.Configuration
{
    public static class AddDefaultRateLimiter
    {
        public static IServiceCollection AddDefRateLim(this IServiceCollection services) {
            services.AddRateLimiter(options =>
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
                }))
            );

            return services;
        }
    }
}
