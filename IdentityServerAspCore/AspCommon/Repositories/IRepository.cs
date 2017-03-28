using AspCommon.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspCommon.Repositories
{
    public interface IRepository
    {
        Task<IEntity> GetAsync(int id);
        Task<IEnumerable<IEntity>> GetAsync();
        Task<IEntity> CreateAsync(IEntity creating);
    }
}
