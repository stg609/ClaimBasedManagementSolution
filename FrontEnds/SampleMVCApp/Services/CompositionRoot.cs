using Common.Domain;
using DryIoc;
using Common.Infra;
using SampleMVCApp.Domain;
using SampleMVCApp.Infra;

namespace SampleMVCApp.Services
{
    public class CompositionRoot
    {
        public CompositionRoot(IRegistrator registrator)
        {
            registrator.Register<IUnitOfWorkManager, UnitOfWorkManager>();
            registrator.Register<IRepository<MenuDTO, string>, GeneralRepository<MenuDTO, string>>();
            registrator.Register<IUnitOfWork, UnitOfWork>();
            registrator.Register<MenuService>();
        }
    }
}
