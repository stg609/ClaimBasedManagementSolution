using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using DryIoc;
using IdServer.Domain;
using Infra;

namespace IdServer.Services
{
    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator registrator)
        {
            registrator.Register<IUnitOfWorkManager, UnitOfWorkManager>();
            registrator.Register<IRepository<ClaimDTO, int>, GeneralRepository<ClaimDTO, int>>();
            registrator.Register<IUnitOfWork, UnitOfWork>();
            registrator.Register<IClaimService, ClaimService>();
        }
    }
}
