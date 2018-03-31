using Common.Domain;
using Common.Infra;
using DryIoc;
using IdServer.Domain;
using IdServer.Infra;
using Microsoft.EntityFrameworkCore;

namespace IdServer.Services
{
    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator registrator)
        {
            registrator.Register<IUnitOfWorkManager, UnitOfWorkManager>(Reuse.InWebRequest);
            registrator.Register<IClaimService, ClaimService>(Reuse.InWebRequest);

            //UnitOfWork will be disposed each time, so it should get a new instance each time
            registrator.Register<IUnitOfWork, UnitOfWork>(setup: Setup.With(allowDisposableTransient: true));
            registrator.Register<IRepository<ClaimDTO, int>, GeneralRepository<ClaimDTO, int>>(setup: Setup.With(allowDisposableTransient: true));
            registrator.Register<DbContext, ApplicationDbContext>(setup: Setup.With(allowDisposableTransient: true));
        }
    }
}
