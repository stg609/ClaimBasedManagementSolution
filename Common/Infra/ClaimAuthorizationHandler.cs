using System.Threading.Tasks;
using Common.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Common.Infra
{
    public class ClaimAuthorizationHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
        {
            if(context.User.Claims.HasPermissionClaim(requirement.Value))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
