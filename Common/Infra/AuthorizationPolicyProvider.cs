using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Common.Infra
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private IConfiguration _configuration;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                //Regex regexClaimPolicy = new Regex(ClaimConstants.PolicyPrefix + "\\.([\\w\\.]+)(\\(\\w+\\))?");
                Regex regexClaimPolicy = new Regex(ClaimConstants.PolicyPrefix + @"\.[\w\.]+[\w\*]+");
                Match match = regexClaimPolicy.Match(policyName);
                if (match.Success)
                {                   
                    policy = new AuthorizationPolicyBuilder()
                        .AddRequirements(new ClaimRequirement(/*ClaimConstants.PermissionClaimType,*/ match.Groups[0].Value))
                        .Build();
                }
            }

            return policy;
        }
    }
}
