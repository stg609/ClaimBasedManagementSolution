using System;
using System.Threading.Tasks;
using Constants;
using Microsoft.AspNetCore.Authorization;

namespace Common.Infra
{
    public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
        {
            if (context.User.HasClaim(ClaimConstants.PermissionClaimType, String.Join(GeneralConstants.DelimeterDot, ClaimConstants.PolicyPrefix, ClaimConstants.ClaimValue_AllowAll)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (!context.User.HasClaim(requirement.Type, requirement.Value))
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
