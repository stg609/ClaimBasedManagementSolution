using System.Collections.Generic;

namespace IdServer.Domain
{
    public interface IClaimService
    {
        List<ClaimDTO> GetAllClaims();
        void AddClaims(IEnumerable<ClaimDTO> claims, string ownerIdentity, string ownerIP);
        void AddClaim(ClaimDTO claim, string ownerIdentity, string ownerIP);
    }
}