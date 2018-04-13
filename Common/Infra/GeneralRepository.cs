using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace Common.Infra
{
    public class GeneralRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        private IUnitOfWork _unitOfWork;

        public GeneralRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(TEntity entity)
        {
            _unitOfWork.Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _unitOfWork.Context.Set<TEntity>().AddRange(entities);
        }    

        public IQueryable<TEntity> GetAll()
        {
            return _unitOfWork.Context.Set<TEntity>();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _unitOfWork.Context.Set<TEntity>().Where(predicate).SingleOrDefault();
        }

        public void Remove(TKey key)
        {
            TEntity existing = _unitOfWork.Context.Set<TEntity>().Find(key);
            if (existing != null)
            {
                _unitOfWork.Context.Set<TEntity>().Remove(existing);
            }
        }

        public void RemoveAll()
        {
            _unitOfWork.Context.Set<TEntity>().RemoveRange(_unitOfWork.Context.Set<TEntity>());
        }

        public void Update(TEntity entity)
        {
            //Only Update the state if the entity is not belong to the DbContext, otherwise the Attach method will cause the attached entity's State become UnChanged.
            if (!_unitOfWork.Context.Exists(entity))
            {
                _unitOfWork.Context.Entry(entity).State = EntityState.Modified;
                _unitOfWork.Context.Set<TEntity>().Attach(entity);
            }
        }
    }
}
