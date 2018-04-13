using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Domain
{
    public interface IRepository<TEntity,TKey> 
         where TEntity : class
    {
        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TKey key);
        void RemoveAll();

        void Update(TEntity entity);
    }
}
