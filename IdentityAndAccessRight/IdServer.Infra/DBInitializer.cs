using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Constants;
using IdServer.Domain;
using Microsoft.AspNetCore.Identity;

namespace IdServer.Infra
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClaimService _claimService;

        public DBInitializer(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IClaimService claimService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _claimService = claimService;
        }

        public async Task Initialize()
        {
            string adminRoleName = "Administrator";

            //TODO remove the EnsureDeleted, it's only for testing
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            if (_context.Roles.Any(itm => itm.Name == adminRoleName)) return;

            await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
            var adminRole = await _roleManager.FindByNameAsync(adminRoleName);

            string superClaim = String.Join(GeneralConstants.DelimeterDot, ClaimConstants.PolicyPrefix, ClaimConstants.ClaimValue_AllowAll);
            _claimService.AddClaim(new ClaimDTO
            {
                Type = ClaimConstants.PermissionClaimType,
                Value = superClaim
            }, "Startup", "0.0.0.0");

            await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimConstants.PermissionClaimType, superClaim));

            string user = "admin@admin.com";
            string password = "Pa55w@rd";
            await _userManager.CreateAsync(new ApplicationUser { UserName = user, Email = user, Nickname = "Administrator", EmailConfirmed = true }, password);
            await _userManager.AddToRoleAsync(_userManager.FindByNameAsync(user).GetAwaiter().GetResult(), adminRoleName);


        }
    }
}
