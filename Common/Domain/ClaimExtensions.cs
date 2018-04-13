using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Common.Constants;

namespace Common.Domain
{
    public static class ClaimExtensions
    {
        public static Claim GetPermissionClaims(this IEnumerable<string> claims)
        {
            if (claims == null || !claims.Any())
            {
                return null;
            }

            return new Claim(ClaimConstants.PermissionClaimType, String.Join(GeneralConstants.DelimeterSemicolon, claims));
        }

        public static bool HasPermissionClaim(this IEnumerable<Claim> claims, string targetClaimValue)
        {
            var permissionClaims = claims.Where(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType));
            foreach (var claim in permissionClaims)
            {
                if(claim.HasPermissionClaim(targetClaimValue))
                {
                    return true;
                }
            }

            return false;
            //if (claims.Any(itm => itm.Type.Equals(ClaimConstants.PermissionClaimType) && itm.Value.Equals(String.Join(".", ClaimConstants.PolicyPrefix, ClaimConstants.ClaimValue_AllowAll))))
            //{
            //    return true;
            //}

            //if (!claims.Any(itm => itm.Type.Equals(targetClaimType) && itm.Value.Equals(targetClaimValue)))
            //{
            //    return false;
            //}

            //return true;
        }

        public static bool HasPermissionClaim(this Claim claim, string targetClaimValue)
        {
            if (claim == null || !claim.Type.Equals(ClaimConstants.PermissionClaimType))
            {
                return false;
            }

            IEnumerable<string> permissionClaims;
            if (String.IsNullOrWhiteSpace(claim.Value))
            {
                return false;
            }

            permissionClaims = claim.Value.Split(GeneralConstants.DelimeterSemicolon);
            if (permissionClaims.Any(itm => itm.Equals(String.Join(GeneralConstants.DelimeterDot, ClaimConstants.PolicyPrefix, ClaimConstants.ClaimValue_AllowAll))))
            {
                return true;
            }

            if (permissionClaims.Any(itm => itm.Equals(targetClaimValue)))
            {
                return true;
            }

            return false;
        }
    }
}
