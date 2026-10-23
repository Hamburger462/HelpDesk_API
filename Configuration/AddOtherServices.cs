using HelpDesk.Mapping;

namespace HelpDesk.Configuration
{
    public static class AddOtherServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(options => options.AddProfile<MainMappingProfile>());

            return services;
        }
    }
}