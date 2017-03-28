using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AuthorizationServer.Attributes
{
    public class SecurityHeaderAttribute : Attribute, IFilterMetadata { }
}
