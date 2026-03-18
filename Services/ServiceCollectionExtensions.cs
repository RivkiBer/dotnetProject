using BakeryServices.Interface;
using NamespaceBakery.Services;
using UserNamespace.Services;
using BakeryNamespace.Services;

namespace BakeryNamespace.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            services.AddPastryService();
            services.AddUserService();
            services.AddActiveUser();
            services.AddNotificationService();
            return services;
        }
    }
}