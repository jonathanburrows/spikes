using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AAuthorizationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorization(this IServiceCollection serviceCollection, Action<AuthorizationServer.Options.AuthorizationOptions> options)
        {
            return serviceCollection
                .AddOptions()
                .Configure(options)
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection serviceCollection) => AddAuthorization(serviceCollection, _ => { });
    }
}
