using System.Linq;
using IdServer.Areas.API.Models;
using IdServer.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Areas.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        private IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpPost]
        public IActionResult SubmitClaims([FromBody]ClaimsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string ownerIp = HttpContext.Connection.RemoteIpAddress.ToString();

                var claims = from claim in model.Claims
                             select new ClaimDTO { Type = claim.Type, Value = claim.Value };

                _claimService.AddClaims(claims, model.Identity, ownerIp);

                return Ok();
            }
            else
            {
                return BadRequest(model);
            }
        }
    }


}