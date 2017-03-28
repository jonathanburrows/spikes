using AspCommon.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspCommon.Repositories
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<TEntity> GetAsync(int id);
        Task<IEnumerable<TEntity>> GetAsync();
        Task<TEntity> CreateAsync(TEntity creating);
    }
}
