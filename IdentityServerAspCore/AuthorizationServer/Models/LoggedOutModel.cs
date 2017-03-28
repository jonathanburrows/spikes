using System.Collections.Generic;

namespace AuthorizationServer.Models
{
    public class LoggedOutModel
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
        public IEnumerable<string> SignoutCallbacks { get; set; }
        public bool AutomaticRedirectAfterSignOut { get; set; }
    }
}
