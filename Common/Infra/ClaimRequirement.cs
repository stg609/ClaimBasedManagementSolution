using Microsoft.AspNetCore.Authorization;

namespace Infra
{
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public ClaimRequirement(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }
        public string Value { get; set; }
    }
}
