using Common.Domain;
using DryIoc;

namespace Common.Infra
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
