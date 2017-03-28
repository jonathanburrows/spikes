using System.Collections.Generic;

namespace AuthorizationServer.Models
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }

        public IEnumerable<ScopeModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeModel> ResourceScopes { get; set; }
    }
}
