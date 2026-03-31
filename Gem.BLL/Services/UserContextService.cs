using Gem.BLL.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Gem.BLL.Services
{
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public string UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string UserName => User?.Identity?.Name;

        public string Role => User?.FindFirst(ClaimTypes.Role)?.Value;

        public bool HasRole(params string[] roles)
        {
            if (string.IsNullOrWhiteSpace(Role) || roles == null || roles.Length == 0)
                return false;

            return roles.Any(r => string.Equals(Role, r, StringComparison.OrdinalIgnoreCase));
        }

    }

}
