using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Constants;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdServer.Domain;
using Microsoft.AspNetCore.Identity;

namespace IdServer.Services
{
    public class IdentityProfileService : IProfileService
    {
        private IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private UserManager<ApplicationUser> _userManager;

        public IdentityProfileService(IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, UserManager<ApplicationUser> userManager)
        {
            _claimsFactory = claimsFactory;
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);

            if (user == null)
            {
                throw new ArgumentException();
            }

            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();
            claims.Add(new Claim(ClaimConstants.NicknameClaimType, user.Nickname));

            //use ; join same permission claim type if existed
            Func<Claim, bool> predicate = itm => itm.Type.Equals(ClaimConstants.PermissionClaimType);
            if (claims.Count(predicate) > 1)
            {
                string jointPermissionClaims = String.Join(GeneralConstants.DelimeterSemicolon, claims.Where(predicate).Select(itm => itm.Value));
                claims.RemoveAll(new Predicate<Claim>(predicate));
                claims.Add(new Claim(ClaimConstants.PermissionClaimType, jointPermissionClaims));
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
