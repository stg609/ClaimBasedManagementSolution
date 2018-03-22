using System.Collections.Generic;

namespace IdServer.Domain
{
    public interface IClaimService
    {
        void AddClaims(IEnumerable<ClaimDTO> claims, string ownerIdentity, string ownerIp);
    }
}