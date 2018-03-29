using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Constants;
using Microsoft.AspNetCore.Identity;

namespace IdServer.Domain
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

        public async Task CreateAsync(string email, string password, IEnumerable<string> roles = null, IEnumerable<string> claims = null, string nickname = null)
        {
            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                Email = email,
                UserName = email,
                Nickname = String.IsNullOrWhiteSpace(nickname) ? email : nickname,
            }, password);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);

                //Add Role
                if (roles != null && roles.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, roles);

                    if (!result.Succeeded)
                    {
                        throw new Exception(GetIdentityErrorMessage(result.Errors));
                    }
                }

                //Add Claim
                if (claims != null && claims.Any())
                {
                    result = await _userManager.AddClaimsAsync(user, claims.Select(itm =>
                    {
                        return new Claim(ClaimConstants.PolicyPrefix, itm);
                    }));

                    if (!result.Succeeded)
                    {
                        throw new Exception(GetIdentityErrorMessage(result.Errors));
                    }
                }
            }
            else
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }
        }

        public async Task UpdateAsync(string email, IEnumerable<string> roles, IEnumerable<string> claims, string password = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("Email is not existed.");
            }

            IdentityResult result = null;

            if (!String.IsNullOrWhiteSpace(password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
                result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    throw new Exception(GetIdentityErrorMessage(result.Errors));
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            roles = roles ?? new List<string>(); //ensure it's not null

            var toDeleteItems = currentRoles.Except(roles);
            var toAddItems = roles.Except(currentRoles);

            //remove roles
            result = await _userManager.RemoveFromRolesAsync(user, toDeleteItems);
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }

            //add roles
            result = await _userManager.AddToRolesAsync(user, toAddItems);
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }

            var currentAdditionalClaims = from itm in await _userManager.GetClaimsAsync(user)
                                          where itm.Type.Equals(ClaimConstants.PermissionClaimType)
                                          select itm.Value;

            var newClaims = claims ?? new List<string>();

            var toDeleteClaims = from itm in currentAdditionalClaims.Except(newClaims)
                                 select new Claim(ClaimConstants.PermissionClaimType, itm);
            var toAddClaims = from itm in newClaims.Except(currentAdditionalClaims)
                              select new Claim(ClaimConstants.PermissionClaimType, itm);

            result = await _userManager.RemoveClaimsAsync(user, toDeleteClaims);
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }

            result = await _userManager.AddClaimsAsync(user, toAddClaims);
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }
        }

        public async Task LockAsync(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("Email doesn't existed.");
            }

            //Lock out 30 mins
            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddMinutes(30));
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }
        }

        public async Task UnLockAsync(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("Email doesn't existed.");
            }

            //Lock out 30 mins
            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                throw new Exception(GetIdentityErrorMessage(result.Errors));
            }
        }
    }
}
