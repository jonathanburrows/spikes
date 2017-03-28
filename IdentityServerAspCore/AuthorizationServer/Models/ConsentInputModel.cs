using System.Collections.Generic;

namespace AuthorizationServer.Models
{
    public class ConsentInputModel
    {
        public bool Confirmed { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
    }
}
