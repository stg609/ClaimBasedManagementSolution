using Common.Domain;
using DryIoc;
using Microsoft.EntityFrameworkCore;

namespace Common.Infra
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Context { get; }

        private IContainer _container;

        public UnitOfWork(DbContext context, IContainer container)
        {
            Context = context;
            _container = container;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            //TODO should use DI way to get real repository
            //return new GeneralRepository<TEntity, TKey>(this);
            return _container.Resolve<IRepository<TEntity, TKey>>();
        }
    }
}
