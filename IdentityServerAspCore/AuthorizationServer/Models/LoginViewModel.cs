using System.Collections.Generic;

namespace AuthorizationServer.Models
{
    public class LoginViewModel : LoginInputModel
    {
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
    }
}
