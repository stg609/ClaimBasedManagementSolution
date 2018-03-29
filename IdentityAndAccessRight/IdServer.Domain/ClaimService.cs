using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain;
using Common.Infra;

namespace IdServer.Domain
{
    public class ClaimService : IClaimService
    {
        private IUnitOfWorkManager _unitOfWorkManager;

        public ClaimService(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public List<ClaimDTO> GetAllClaims()
        {
            IUnitOfWork uow = null;
            using (uow = _unitOfWorkManager.Begin())
            {
                return uow.GetRepository<ClaimDTO, int>().GetAll()?.ToList().EmptyListIfEmpty();
            }
        }

        public void AddClaims(IEnumerable<ClaimDTO> claims, string ownerIdentity, string ownerIP)
        {
            if (claims == null || !claims.Any())
            {
                throw new ArgumentNullException(nameof(claims));
            }

            if (String.IsNullOrWhiteSpace(ownerIdentity))
            {
                throw new ArgumentNullException(nameof(ownerIdentity));
            }

            if (String.IsNullOrWhiteSpace(ownerIP))
            {
                throw new ArgumentNullException(nameof(ownerIP));
            }

            IUnitOfWork uow = null;

            using (uow = _unitOfWorkManager.Begin())
            {
                var repo = uow.GetRepository<ClaimDTO, int>();

                var claimsWithOwnerInfo = claims.Select(itm =>
                {
                    itm.OwnerIdentity = ownerIdentity;
                    itm.OwnerIP = ownerIP;
                    return itm;
                });

                repo.AddRange(claimsWithOwnerInfo);
                uow.Commit();
            }
        }

        public void AddClaim(ClaimDTO claim, string ownerIdentity, string ownerIP)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (String.IsNullOrWhiteSpace(ownerIdentity))
            {
                throw new ArgumentNullException(nameof(ownerIdentity));
            }

            if (String.IsNullOrWhiteSpace(ownerIP))
            {
                throw new ArgumentNullException(nameof(ownerIP));
            }

            IUnitOfWork uow = null;

            using (uow = _unitOfWorkManager.Begin())
            {
                var repo = uow.GetRepository<ClaimDTO, int>();

                claim.OwnerIdentity = ownerIdentity;
                claim.OwnerIP = ownerIP;
                repo.Add(claim);
                uow.Commit();
            }
        }
    }
}
