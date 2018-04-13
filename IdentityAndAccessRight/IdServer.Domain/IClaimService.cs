using System.Collections.Generic;

namespace IdServer.Domain
{
    public interface IClaimService
    {
        List<ClaimDTO> GetAllClaims();
        void AddOrUpdateClaims(IEnumerable<ClaimDTO> claims, string ownerIdentity, string ownerIP);
        void AddOrUpdateClaim(ClaimDTO claim, string ownerIdentity, string ownerIP);
    }
}