using System;
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
            //Ensure use the same UnitOfWork Instance 
            var getRepository = _container.Resolve<Func<IUnitOfWork, IRepository<TEntity, TKey>>>();
            return getRepository(this);
        }
    }
}
