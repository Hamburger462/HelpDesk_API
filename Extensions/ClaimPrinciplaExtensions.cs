using System.Security.Claims;

namespace HelpDesk.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Id of the authenticated user, or null when the request is anonymous or the claim is missing/invalid.
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? principal.FindFirst("sub")?.Value;

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}