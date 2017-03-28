using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspCommon.Models;
using System.Linq;

namespace AspCommon.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>, IRepository where TEntity : IEntity
    {
        private static IDictionary<int, TEntity> entities { get; } = new Dictionary<int, TEntity>();

        public Task<TEntity> GetAsync(int id)
        {
            return Task.FromResult(entities[id]);
        }

        async Task<IEntity> IRepository.GetAsync(int id)
        {
            return await GetAsync(id);
        }

        public Task<IEnumerable<TEntity>> GetAsync()
        {
            return Task.FromResult(entities.Values.AsEnumerable());
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAsync()
        {
            return (IEnumerable<IEntity>)await GetAsync();
        }

        public Task<TEntity> CreateAsync(TEntity creating)
        {
            var id = entities.Any()? entities.Values.Max(e => e.Id) + 1 : 1;
            creating.Id = id;
            entities[id] = creating;
            return Task.FromResult(creating);
        }

        async Task<IEntity> IRepository.CreateAsync(IEntity creating)
        {
            return await CreateAsync((TEntity)creating);
        }
    }
}
