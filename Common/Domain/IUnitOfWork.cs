using System;
using Microsoft.EntityFrameworkCore;

namespace Common.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        void Commit();

        IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;
    }
}
