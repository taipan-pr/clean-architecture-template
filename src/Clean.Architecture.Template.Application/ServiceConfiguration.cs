using Microsoft.Extensions.DependencyInjection;

namespace Clean.Architecture.Template.Application
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services;
        }
    }
}
