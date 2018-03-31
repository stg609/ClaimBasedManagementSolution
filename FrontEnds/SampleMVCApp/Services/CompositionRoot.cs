using Common.Domain;
using Common.Infra;
using DryIoc;
using Microsoft.EntityFrameworkCore;
using SampleMVCApp.Domain;
using SampleMVCApp.Infra;

namespace SampleMVCApp.Services
{
    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator registrator)
        {
            registrator.Register<IUnitOfWorkManager, UnitOfWorkManager>(Reuse.InWebRequest);
            registrator.Register<IMenuService, MenuService>(Reuse.InWebRequest);

            //UnitOfWork will be disposed each time, so it should get a new instance each time
            registrator.Register<IUnitOfWork, UnitOfWork>(setup: Setup.With(allowDisposableTransient: true));
            registrator.Register<IRepository<MenuDTO, int>, GeneralRepository<MenuDTO, int>>(setup: Setup.With(allowDisposableTransient: true));
            registrator.Register<DbContext, ApplicationDbContext>(setup: Setup.With(allowDisposableTransient: true));

        }
    }
}
