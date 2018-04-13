using System;

namespace Constants
{
    public class ClaimConstants
    {
        public const string PolicyPrefix = "CLAIM";
        public const string ClaimValue_AllowAll = "*";
        public const string PermissionClaimType = "Permission";
        public const string NicknameClaimType = "Nickname";
        public const string CLAIM_REGULAR_PATTERN = PolicyPrefix + @"\.[\w\.]+[\w\*]+";

        public const string ASPNETIDENTITY_NAME = "name";
    }
}
