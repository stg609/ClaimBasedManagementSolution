using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace IdServer.Domain
{
    public class ClaimService : IClaimService
    {
        private IUnitOfWorkManager _unitOfWorkManager;

        public ClaimService(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void AddClaims(IEnumerable<ClaimDTO> claims, string ownerIdentity, string ownerIp)
        {
            if (claims == null || !claims.Any())
            {
                throw new ArgumentNullException(nameof(claims));
            }

            if (String.IsNullOrWhiteSpace(ownerIdentity))
            {
                throw new ArgumentNullException(nameof(ownerIdentity));
            }

            if (String.IsNullOrWhiteSpace(ownerIp))
            {
                throw new ArgumentNullException(nameof(ownerIp));
            }

            IUnitOfWork uow = null;

            using (uow = _unitOfWorkManager.Begin())
            {
                var repo = uow.GetRepository<ClaimDTO, int>();

                var claimsWithOwnerInfo = claims.Select(itm =>
                {
                    itm.OwnerIdentity = ownerIdentity;
                    itm.OwnerIp = ownerIp;
                    return itm;
                });

                repo.AddRange(claimsWithOwnerInfo);
                uow.Commit();
            }
        }
    }
}
