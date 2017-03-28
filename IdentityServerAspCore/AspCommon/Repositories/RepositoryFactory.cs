using AspCommon.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace AspCommon.Repositories
{
    public class RepositoryFactory
    {
        private IServiceProvider ServiceProvider { get; }

        private static MethodInfo GenericConstructMethod { get; } = typeof(RepositoryFactory).GetTypeInfo().GetMethod(nameof(RepositoryFactory.Construct), new Type[0]);

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IRepository<TEntity> Construct<TEntity>() where TEntity : IEntity
        {
            return ServiceProvider.GetRequiredService<IRepository<TEntity>>();
        }

        public IRepository Construct(Type entityType) {
            var castedConstructMethod = GenericConstructMethod.MakeGenericMethod(entityType);
            var constructed = (IRepository)castedConstructMethod.Invoke(this, new object[0]);
            return constructed;
        }
    }
}
