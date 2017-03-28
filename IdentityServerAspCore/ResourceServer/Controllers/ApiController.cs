using AspCommon.Models;
using AspCommon.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ResourceServer.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ApiController : ControllerBase
    {
        private RepositoryFactory RepositoryFactory { get; }

        public ApiController(RepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        [HttpGet("{entityName}/{id}")]
        public async Task<IEntity> Get(string entityName, int id)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var type = assembly.ExportedTypes.Single(x => x.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
            var repositoryType = typeof(IRepository<>).MakeGenericType(type);
            var repository = RepositoryFactory.Construct(type);

            return await repository.GetAsync(id);
        }

        [HttpGet("{entityName}")]
        public async Task<IEnumerable<IEntity>> Get(string entityName)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var type = assembly.ExportedTypes.Single(x => x.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
            var repository = RepositoryFactory.Construct(type);

            return await repository.GetAsync();
        }
    }
}
