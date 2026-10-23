using HelpDesk.Mapping;
using HelpDesk.Services.Storage;

namespace HelpDesk.Configuration
{
    public static class AddOtherServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(options => options.AddProfile<MainMappingProfile>());
            services.AddSingleton<IFileStorage, LocalFileStorage>();

            return services;
        }
    }
}