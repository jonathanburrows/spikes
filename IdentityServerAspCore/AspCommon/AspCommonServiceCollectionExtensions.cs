using AspCommon.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspCommonServiceCollectionExtensions
    {
        public static IServiceCollection AddAspCommon(this IServiceCollection serviceCollection) {
            return serviceCollection
                .AddScoped<RepositoryFactory>()
                .AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
