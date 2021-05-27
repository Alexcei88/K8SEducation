using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication.Requirements;
using OTUS.HomeWork.RestAPI.Abstraction.DAL;

namespace OTUS.HomeWork.EShop.Authentication
{
    public class PermissionHandler
        : AuthorizationHandler<OwnerPermission>
    {
        private readonly UserRepository _repository;

        public PermissionHandler(UserRepository repository)
        {
            _repository = repository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerPermission requirement)
        {
            if (await IsOwner(context.User, context.Resource))
            {
                context.Succeed(requirement);
            }
        }

        private async Task<bool> IsOwner(ClaimsPrincipal user, object resource)
        {
            DefaultHttpContext context = resource as DefaultHttpContext;
            if (context == null)
                return false;

            // Code omitted for brevity
            if(context.Request.Path.StartsWithSegments("/api/user")
                || context.Request.Path.StartsWithSegments("/api/bucket")
                || context.Request.Path.StartsWithSegments("/api/order"))
            {
                var name = user.FindFirst(ClaimTypes.Name);
                var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier);
                if (nameIdentifier == null && name == null)
                    return false;

                var userId = nameIdentifier?.Value;
                if (userId == null)
                {
                    var dbUser = await _repository.GetUserAsync(name.Value);
                    if (dbUser == null)
                        return false;
                    userId = dbUser.Id.ToString();
                }
                string userIdFromPath = context.Request.Path.Value.Split('/')[3];
                return userIdFromPath.Equals(userId.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }
    }
}
