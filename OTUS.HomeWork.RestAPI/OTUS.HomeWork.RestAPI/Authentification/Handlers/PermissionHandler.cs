﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OTUS.HomeWork.RestAPI.Authentification.Requirements;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OTUS.HomeWork.RestAPI.Authentification.Handlers
{
    internal class PermissionHandler
        : AuthorizationHandler<OwnerPermission>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerPermission requirement)
        {
            if (IsOwner(context.User, context.Resource))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool IsOwner(ClaimsPrincipal user, object resource)
        {
            DefaultHttpContext context = resource as DefaultHttpContext;
            if (context == null)
                return false;

            // Code omitted for brevity
            if(context.Request.Path.StartsWithSegments("/api/user"))
            {
                var nameIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (nameIdentifier == null)
                    return false;

                string userIdFromPath = context.Request.Path.Value.Split('/').Last();
                return userIdFromPath.Equals(nameIdentifier, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }
    }
}
