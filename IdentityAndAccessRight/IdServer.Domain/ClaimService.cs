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

        public void AddOrUpdateClaims(IEnumerable<ClaimDTO> claims, string ownerIdentity, string ownerIP)
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

                //如果 ownerIdentity 与 ownerIp 相同认为是同一个客户，同一个客户的同一个 claimType 如果已经存在则只修改。
                var existedClaims = repo.GetAll().Where(itm => itm.OwnerIdentity == ownerIdentity && itm.OwnerIP == ownerIP).ToList();

                var claimsWithOwnerInfo = claims.Select(itm =>
                {
                    itm.OwnerIdentity = ownerIdentity;
                    itm.OwnerIP = ownerIP;
                    return itm;
                });

                foreach (var claim in claimsWithOwnerInfo)
                {
                    var existedClaim = existedClaims.SingleOrDefault(itm => itm.Type == claim.Type);
                    if (existedClaim != null)
                    {
                        existedClaim.Value = claim.Value;
                    }
                    else
                    {
                        repo.Add(claim);

                    }
                }
                uow.Commit();
            }
        }

        public void AddOrUpdateClaim(ClaimDTO claim, string ownerIdentity, string ownerIP)
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

                //如果 ownerIdentity 与 ownerIp 相同认为是同一个客户，同一个客户的同一个 claimType 如果已经存在则只修改。
                var existedClaim = repo.Get(itm => itm.Type == claim.Type && itm.OwnerIdentity == ownerIdentity && itm.OwnerIP == ownerIP);
                if (existedClaim != null)
                {
                    existedClaim.Value = claim.Value;
                }
                else
                {
                    repo.Add(claim);
                }
                uow.Commit();
            }
        }
    }
}
