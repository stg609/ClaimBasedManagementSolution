using System;
using System.Linq;
using System.Security.Claims;
using Constants;

namespace SampleMVCApp.Services
{
    public static class Extensions
    {
        public static string GetNickname(this ClaimsPrincipal principal)
        {
            var claim = principal.Claims.FirstOrDefault(itm => itm.Type.Equals(ClaimConstants.NicknameClaimType));
            if(claim!=null)
            {
                return claim.Value;
            }

            return String.Empty;
        }
    }
}
