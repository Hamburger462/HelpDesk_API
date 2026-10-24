using System.Security.Claims;
using HelpDesk.Data.Models;

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

        // Operators and admins work with everyone's tickets; regular users only with their own.
        public static bool IsStaff(this ClaimsPrincipal principal) =>
            principal.IsInRole(Roles.Operator) || principal.IsInRole(Roles.Admin);
    }
}
