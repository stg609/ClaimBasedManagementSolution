using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Constants;
using Microsoft.AspNetCore.Identity;

namespace IdServer.Domain
{
    public class RoleService : IRoleService
    {
        private RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        private string GetIdentityErrorMessage(IEnumerable<IdentityError> errors)
        {
            if (errors == null)
            {
                return String.Empty;
            }

            return String.Join(Environment.NewLine, from err in errors
                                                    select err.Description);
        }

        public async Task CreateAsync(string name, IEnumerable<string> claims = null)
        {
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole { Name = name });
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }
            else
            {
                if (claims != null && claims.Any())
                {
                    var role = await _roleManager.FindByNameAsync(name);
                    foreach (var claim in claims)
                    {
                        result = await _roleManager.AddClaimAsync(role, new Claim(ClaimConstants.PermissionClaimType, claim));
                        if (!result.Succeeded)
                        {
                            throw new Exception(GetIdentityErrorMessage(result.Errors));
                        }
                    }
                }
            }
        }

        public async Task UpdateAsync(string name, IEnumerable<string> claims)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role == null)
            {
                throw new ArgumentException("Role is not existed.");
            }

            IdentityResult result = null;

            var currentClaims = from itm in await _roleManager.GetClaimsAsync(role)
                                where itm.Type.Equals(ClaimConstants.PermissionClaimType)
                                select itm.Value;

            claims = claims ?? new List<string>(); //ensure it's not null

            var toDeleteItems = from itm in currentClaims.Except(claims)
                                select new Claim(ClaimConstants.PermissionClaimType, itm);
            var toAddItems = from itm in claims.Except(currentClaims)
                             select new Claim(ClaimConstants.PermissionClaimType, itm);

            //remove claims
            foreach (var itm in toDeleteItems)
            {
                result = await _roleManager.RemoveClaimAsync(role, itm);
                if (!result.Succeeded)
                {
                    throw new Exception(GetIdentityErrorMessage(result.Errors));
                }
            }

            //add claims
            foreach (var claim in toAddItems)
            {
                result = await _roleManager.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    throw new Exception(GetIdentityErrorMessage(result.Errors));
                }
            }
        }
    }
}
