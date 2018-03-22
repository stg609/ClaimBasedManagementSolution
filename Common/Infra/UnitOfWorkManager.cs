using Domain;
using DryIoc;

namespace Infra
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private IContainer _container;

        public UnitOfWorkManager(IContainer container)
        {
            _container = container;
        }

        public IUnitOfWork Begin()
        {
            return _container.Resolve<IUnitOfWork>();
        }
    }
}
