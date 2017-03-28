using AuthorizationServer.Models;
using System;
using System.Collections.Generic;

namespace AuthorizationServer.Options
{
    public class AuthorizationOptions
    {
        public IList<ExternalProvider> WindowsProviders { get; set; } = new List<ExternalProvider>();
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30);
        public bool AutomaticRedirectionAfterSignOut { get; set; } = true;
        public bool ShowLogoutPrompt { get; set; }
    }
}
