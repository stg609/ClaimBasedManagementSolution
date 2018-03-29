using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants;
using IdServer.Domain;
using IdServer.Models;
using IdServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Controllers
{
    [Authorize(Policy = ClaimConstants.PolicyPrefix + "." + IdServerConstants.ID_SERVER_IDENTITY + ".Claims")]
    public class ClaimsController : Controller
    {
        private IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }
        public IActionResult Index()
        {
            IEnumerable<ClaimViewModel> coll = from itm in _claimService.GetAllClaims()
                                               select new ClaimViewModel
                                               {
                                                   Key = itm.Key,
                                                   Value = itm.Value,
                                                   OwnerIdentity = itm.OwnerIdentity,
                                                   OwnerIp = itm.OwnerIP
                                               };

            return View(coll);
        }
    }
}