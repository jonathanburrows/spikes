using AuthorizationServer.Attributes;
using AuthorizationServer.Models;
using AuthorizationServer.Options;
using AuthorizationServer.Stores;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    [SecurityHeader]
    public class AccountController : Controller
    {
        private IIdentityServerInteractionService InteractionService { get; }
        private IClientStore ClientStore { get; }
        private TestUserStore TestUserStore { get; }
        private TestStore TestStore { get; }
        private AuthorizationOptions AuthorizationOptions { get; }

        public AccountController(IIdentityServerInteractionService interactionService, IClientStore clientStore, TestUserStore testUserStore, IOptions<AuthorizationOptions> authorizationOptions, TestStore testStore)
        {
            if (interactionService == null)
            {
                throw new ArgumentNullException(nameof(interactionService));
            }
            InteractionService = interactionService;

            if (clientStore == null)
            {
                throw new ArgumentNullException(nameof(clientStore));
            }
            ClientStore = clientStore;

            if (testUserStore == null)
            {
                throw new ArgumentNullException(nameof(testUserStore));
            }
            TestUserStore = testUserStore;

            if (authorizationOptions == null || authorizationOptions.Value == null)
            {
                throw new ArgumentNullException(nameof(authorizationOptions));
            }
            AuthorizationOptions = authorizationOptions.Value;

            if (testStore == null)
            {
                throw new ArgumentNullException(nameof(testStore));
            }
            TestStore = testStore;
        }

        /// <summary>
        /// Returns a view for the user to login
        /// </summary>
        /// <param name="returnUrl">The location to return to.</param>
        /// <returns>Markup for logining in</returns>
        /// <exception cref="ArgumentNullException"><paramref name="returnUrl"/> is null.</exception>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (returnUrl == null)
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }
            if (!InteractionService.IsValidReturnUrl(returnUrl))
            {
                throw new ArgumentException($"'{returnUrl}' is not a valid return url.");
            }

            var authorizationContext = await InteractionService.GetAuthorizationContextAsync(returnUrl);
            var externalProviders = await GetProvidersAsync(authorizationContext);
            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders,
                Username = authorizationContext?.LoginHint
            };

            return View(nameof(Login), viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (!InteractionService.IsValidReturnUrl(model.ReturnUrl))
            {
                throw new InvalidOperationException($"The url of '{model.ReturnUrl}' is not a valid return url.");
            }

            if (ModelState.IsValid)
            {
                if (TestUserStore.ValidateCredentials(model.Username, model.Password))
                {
                    var expiry = DateTimeOffset.UtcNow.Add(AuthorizationOptions.RefreshTokenLifetime);
                    var props = model.RememberLogin ?
                        new AuthenticationProperties { ExpiresUtc = expiry, IsPersistent = true } :
                        null;

                    var user = TestUserStore.FindByUsername(model.Username);
                    await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, props);

                    return Redirect(model.ReturnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            var authorizationContext = await InteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
            var externalProviders = await GetProvidersAsync(authorizationContext);
            var viewModel = new LoginViewModel
            {
                Username = model.Username,
                Password = model.Password,
                RememberLogin = model.RememberLogin,
                ReturnUrl = model.ReturnUrl,
                ExternalProviders = externalProviders
            };
            return View(nameof(Login), viewModel);
        }

        private async Task<IEnumerable<ExternalProvider>> GetProvidersAsync(AuthorizationRequest authorizationContext)
        {
            if (authorizationContext?.IdP != null)
            {
                return new[]
                {
                    new ExternalProvider { AuthenticationScheme = authorizationContext.IdP }
                };
            }

            var schemes = HttpContext.Authentication.GetAuthenticationSchemes();

            var windowsProviders = AuthorizationOptions
                .WindowsProviders
                .Where(p => schemes.Any(s => s.AuthenticationScheme.Equals(p.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)));

            var oauthProviders = schemes
                .Where(s => s.DisplayName != null)
                .Select(s => new ExternalProvider { AuthenticationScheme = s.AuthenticationScheme, DisplayName = s.DisplayName });

            var providers = windowsProviders.Union(oauthProviders);

            // if the client has restrictions, apply filters on how it can log in.
            if (authorizationContext?.ClientId != null)
            {
                var client = await ClientStore.FindEnabledClientByIdAsync(authorizationContext.ClientId);
                if (client?.IdentityProviderRestrictions?.Any() == true)
                {
                    // filter the authentication scheme.
                    return providers.Where(p => client.IdentityProviderRestrictions.Contains(p.AuthenticationScheme));
                }
            }

            return providers;
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var model = new LogoutModel { LogoutId = logoutId };

            if (!await ShouldShowLogoutAsync(logoutId))
            {
                return await Logout(model);
            }
            else
            {
                return View(nameof(Logout), model);
            }
        }

        private async Task<bool> ShouldShowLogoutAsync(string logoutId)
        {
            if (!AuthorizationOptions.ShowLogoutPrompt)
            {
                return false;
            }

            var user = await HttpContext.GetIdentityServerUserAsync();
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var context = await InteractionService.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt != true)
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> Logout(LogoutModel model)
        {
            var logout = await InteractionService.GetLogoutContextAsync(model.LogoutId);
            var automaticRedirect = AuthorizationOptions.AutomaticRedirectionAfterSignOut && logout?.SignOutIFrameUrl != null;
            var signoutCallbacks = TestStore
                .GetClients()
                .Where(client => client.LogoutUri != null)
                .Select(client => client.LogoutUri)
                .Where(logoutUrl =>
                {
                    if (logout?.PostLogoutRedirectUri == null)
                    {
                        return true;
                    }
                    else if (logoutUrl == null)
                    {
                        return false;
                    }
                    else
                    {
                        var logoutUri = new Uri(logoutUrl);
                        var logoutDomain = logoutUri.IsDefaultPort ? $"{logoutUri.Scheme}://{logoutUri.Host}" : $"{logoutUri.Scheme}://{logoutUri.Host}:{logoutUri.Port}";

                        var postLogoutRedirectUri = new Uri(logout.PostLogoutRedirectUri);
                        var postLogoutDomain = postLogoutRedirectUri.IsDefaultPort ? $"{postLogoutRedirectUri.Scheme}://{postLogoutRedirectUri.Scheme}" : $"{postLogoutRedirectUri.Scheme}://{postLogoutRedirectUri.Scheme}:{postLogoutRedirectUri.Port}";

                        return !logoutDomain.Equals(postLogoutDomain, StringComparison.OrdinalIgnoreCase);
                    }
                });

            var viewModel = new LoggedOutModel
            {
                AutomaticRedirectAfterSignOut = automaticRedirect,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignoutCallbacks = signoutCallbacks,
                //SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            var externalProvider = await GetExternalProviderAsync();
            if (externalProvider != null)
            {
                var logoutId = model.LogoutId ?? await InteractionService.CreateLogoutContextAsync();
                var url = Url.Action(nameof(Logout), new { logotId = model.LogoutId });

                try
                {
                    var logoutProperties = new AuthenticationProperties { RedirectUri = url };
                    await HttpContext.Authentication.SignOutAsync(externalProvider, logoutProperties);
                }
                catch (NotSupportedException)
                {
                    // hacks for external providers which dont have signout.
                }
                catch (InvalidOperationException)
                {
                    //hack for windows negotiate.
                }

            }

            // dlete the local authentication cooie
            await HttpContext.Authentication.SignOutAsync();

            return View("LoggedOut", viewModel);
        }

        private async Task<string> GetExternalProviderAsync()
        {
            var user = await HttpContext.GetIdentityServerUserAsync();
            var idp = user?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                return idp;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var callbackUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl = returnUrl });

            var windowsProvider = AuthorizationOptions.WindowsProviders.FirstOrDefault(wp => wp.AuthenticationScheme.Equals(provider, StringComparison.OrdinalIgnoreCase));
            if (windowsProvider == null)
            {
                var thirdPartyAuthenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = callbackUrl,
                    Items = { { "scheme", provider } }
                };
                return new ChallengeResult(provider, thirdPartyAuthenticationProperties);
            }
            else if (User is WindowsPrincipal)
            {
                var windowsAuthenticationProperties = new AuthenticationProperties
                {
                    Items =
                    {
                        ["scheme"] = windowsProvider.AuthenticationScheme
                    }
                };

                var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, User.Identity.Name),
                    new Claim(ClaimTypes.Name, User.Identity.Name)
                });
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.Authentication.SignInAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme, claimsPrincipal, windowsAuthenticationProperties);
                return Redirect(callbackUrl);
            }
            else
            {
                var windowsProviders = AuthorizationOptions.WindowsProviders.Select(wp => wp.AuthenticationScheme).ToList();
                return new ChallengeResult(windowsProviders);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            if (returnUrl == null)
            {
                throw new ArgumentNullException(nameof(returnUrl));
            }
            if (!InteractionService.IsValidReturnUrl(returnUrl))
            {
                throw new ArgumentException($"return url {returnUrl} is invalid.");
            }

            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new InvalidOperationException("External authentication error.");
            }

            var claims = tempUser.Claims.ToList();

            var subject = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            var nameIdentifier = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userIdClaim = subject ?? nameIdentifier;
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("unknown userid");
            }

            claims.Remove(userIdClaim);
            var provider = info.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            var user = TestUserStore.FindByExternalProvider(provider, userId) ?? TestUserStore.AutoProvisionUser(provider, userId, claims);

            var sessionClaims = claims.Where(c => c.Type == JwtClaimTypes.SessionId).ToArray();

            var idToken = info.Properties.GetTokenValue("id_token");
            var props = idToken != null ? new AuthenticationProperties(new Dictionary<string, string> { ["id_token"] = idToken }) : null;

            await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, provider, props, sessionClaims);
            await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            return Redirect(returnUrl);
        }
    }
}
