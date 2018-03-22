using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SampleMVCApp.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        void Commit();

        IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;
    }
}
