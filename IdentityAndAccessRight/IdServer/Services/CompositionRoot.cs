using Common.Domain;
using Common.Infra;
using DryIoc;
using IdServer.Domain;

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
