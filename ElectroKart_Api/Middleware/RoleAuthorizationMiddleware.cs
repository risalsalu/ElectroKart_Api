using ElectroKart_Api.Attributes;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;

namespace ElectroKart_Api.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<AuthorizeRoleAttribute>();

            // If the endpoint doesn't have our attribute, let the request continue
            if (attribute == null)
            {
                await _next(context);
                return;
            }

            // Check if user is authenticated
            if (context.User.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // Check if user has the required role
            if (!context.User.HasClaim(ClaimTypes.Role, attribute.Role))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            // User has the role, let the request continue
            await _next(context);
        }
    }
}