using AuthorizationServer.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Middleware
{
    public class SecurityHeaderMiddleware
    {
        private RequestDelegate Next { get; }
        private IActionContextAccessor ActionContextAccessor { get; }

        public SecurityHeaderMiddleware(RequestDelegate next, IActionContextAccessor actionContextAccessor)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            Next = next;

            if (actionContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(actionContextAccessor));
            }
            ActionContextAccessor = actionContextAccessor;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var actionDescriptor = ActionContextAccessor?.ActionContext?.ActionDescriptor;
            var filters = actionDescriptor?.FilterDescriptors?.Select(fd => fd.Filter);
            if (filters?.Any(f => f is SecurityHeaderAttribute) == true)
            {
                var headers = httpContext.Response.Headers;

                var contentTypeOptions = "X-Content-Type-Options";
                if (!headers.ContainsKey(contentTypeOptions))
                {
                    headers.Add(contentTypeOptions, "nosniff");
                }

                var frameOptions = "X-Frame-Options";
                if (!headers.ContainsKey(frameOptions))
                {
                    headers.Add(frameOptions, "SAMEORIGIN");
                }

                var csp = "default-src 'self'";

                var contentSecurityPolicy = "Content-Security-Policy";
                if (!headers.ContainsKey(contentSecurityPolicy))
                {
                    headers.Add(contentSecurityPolicy, csp);
                }

                var xContentSecurityPolicy = "X-Content-Security-Policy";
                if (!headers.ContainsKey(xContentSecurityPolicy))
                {
                    headers.Add(xContentSecurityPolicy, csp);
                }
            }

            await Next.Invoke(httpContext);
        }
    }
}
