﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SampleMVCApp.Domain
{
    public interface IRepository<TEntity,TKey> 
         where TEntity : class
    {
        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void Update(TEntity entity);
    }
}