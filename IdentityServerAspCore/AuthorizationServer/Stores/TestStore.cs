using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using static IdentityServer4.IdentityServerConstants.StandardScopes;

namespace AuthorizationServer.Stores
{
    public class TestStore
    {
        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("resource-server", "Resource Server")
            };
        }

        public IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "access-code",
                    ClientName = "Authorization Code Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = false,
                    AllowOfflineAccess = true,

                    RedirectUris = { "https://localhost:44302/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44302/signout-callback-oidc" },
                    AllowedCorsOrigins = { "https://localhost:44302" },
                    LogoutUri = "https://localhost:44302/logout-local",
                    UpdateAccessTokenClaimsOnRefresh = true,

                    AllowedScopes =
                    {
                        "resource-server",
                        OpenId,
                        Profile
                    }
                },

                new Client
                {
                    ClientId = "implicit-client",
                    ClientName = "Implicit Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,

                    RedirectUris = { "http://localhost:5003/account/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5003" },
                    AllowedCorsOrigins = { "http://localhost:5003" },
                    LogoutUri = "http://localhost:5003/account/logout-local",
                    AllowedScopes =
                    {
                        "resource-server",
                        OpenId,
                        Profile
                    }
                },

                new Client
                {
                    ClientId = "resource-owner-client",
                    ClientName = "Resource Owner Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = new [] { GrantType.ResourceOwnerPassword, GrantType.AuthorizationCode },
                    //AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,

                    RequireConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RedirectUris = { "http://localhost:5004" },
                    PostLogoutRedirectUris = { "http://localhost:5004" },
                    AllowedCorsOrigins = { "http://localhost:5004" },
                    AllowedScopes =
                    {
                        OpenId,
                        OfflineAccess,
                        Profile,
                        "resource-server"
                    }
                }
            };
        }

        public List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "testuser",
                    Password = "password",
                    Claims =
                    {
                        new Claim("name", "jonathan burrows")
                    }
                }
            };
        }
    }
}
