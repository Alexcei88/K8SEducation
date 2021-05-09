using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
                var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier);
                if (nameIdentifier == null)
                    return false;

                var dbUser = await _repository.GetUserAsync(nameIdentifier.Value);
                if (dbUser == null)
                    return false;

                string userIdFromPath = context.Request.Path.Value.Split('/')[3];
                return userIdFromPath.Equals(dbUser.Id.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }
    }
}
