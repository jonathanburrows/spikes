using AuthorizationServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants.StandardScopes;

namespace AuthorizationServer.Controllers
{
    public class ConsentController : Controller
    {
        private IIdentityServerInteractionService InteractionService { get; }
        private IClientStore ClientStore { get; }
        private IResourceStore ResourceStore { get; }

        public ConsentController(IIdentityServerInteractionService interactionService, IClientStore clientStore, IResourceStore resourceStore)
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

            if (resourceStore == null)
            {
                throw new ArgumentNullException(nameof(resourceStore));
            }
            ResourceStore = resourceStore;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var viewModel = await BuildConsentViewModel(returnUrl);
            return View(nameof(Index), viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (model.ReturnUrl == null)
            {
                throw new ArgumentNullException(nameof(model.ReturnUrl));
            }

            if (!model.Confirmed)
            {
                var request = await InteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                await InteractionService.GrantConsentAsync(request, ConsentResponse.Denied);
                return Redirect(model.ReturnUrl);
            }
            else if (model.ScopesConsented?.Any() == true)
            {
                var grantedConsent = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent,
                    ScopesConsented = model.ScopesConsented
                };
                var request = await InteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                await InteractionService.GrantConsentAsync(request, grantedConsent);
                return Redirect(model.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "You must pick at least one permission");
                var viewModel = await BuildConsentViewModel(model.ReturnUrl);
                return View(nameof(Index), viewModel);
            }
        }

        private async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
        {
            var request = await InteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                throw new InvalidOperationException($"InteractionService.GetAuthorizationContextAsync('{returnUrl}') returned null.");
            }

            var client = await ClientStore.FindEnabledClientByIdAsync(request.ClientId);
            if (client == null)
            {
                throw new InvalidOperationException($"ClientStore.FindEnabledClientByIdAsync(clientid) returned null.");
            }

            var resources = await ResourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            if (resources == null)
            {
                var scopes = string.Join(", ", request.ScopesRequested);
                throw new InvalidOperationException($"ResourceStore.FindEnabledResourcesByScopeAsync({scopes}) returned null.");
            }
            if (!resources.IdentityResources.Any() && !resources.ApiResources.Any())
            {
                var scopes = string.Join(", ", request.ScopesRequested);
                throw new InvalidOperationException($"No scopes matching: {scopes}");
            }

            var identityScopes = resources.IdentityResources.Select(ScopeModelForIdentityResource);

            var offlineScopes = resources.OfflineAccess ? new[] { OfflineAccessScope() } : new ScopeModel[0];
            var resourceScopes = resources.ApiResources
                .SelectMany(r => r.Scopes)
                .Select(ScopeModelForScope)
                .Union(offlineScopes);

            return new ConsentViewModel
            {
                RememberConsent = true,
                ScopesConsented = Enumerable.Empty<string>(),
                ReturnUrl = returnUrl,
                ClientName = client.ClientName,
                ClientUrl = client.ClientUri,
                ClientLogoUrl = client.LogoUri,
                IdentityScopes = identityScopes,
                ResourceScopes = resourceScopes
            };
        }

        private static ScopeModel ScopeModelForIdentityResource(IdentityResource identityResource)
        {
            return new ScopeModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Emphasize = identityResource.Emphasize,
                Required = identityResource.Required,
                Checked = true
            };
        }

        private static ScopeModel ScopeModelForScope(Scope scope)
        {
            return new ScopeModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = true
            };
        }

        private static ScopeModel OfflineAccessScope()
        {
            return new ScopeModel
            {
                Name = OfflineAccess,
                DisplayName = "Offline Access",
                Description = "Access to your applications and resources, even when you are offline",
                Emphasize = true,
                Checked = true
            };
        }
    }
}
