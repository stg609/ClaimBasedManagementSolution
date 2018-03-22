using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infra
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

        public IEnumerable<TEntity> GetAll()
        {
            return _unitOfWork.Context.Set<TEntity>().AsEnumerable<TEntity>();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _unitOfWork.Context.Set<TEntity>().Where(predicate).SingleOrDefault();
        }

        public void Remove(TEntity entity)
        {
            TEntity existing = _unitOfWork.Context.Set<TEntity>().Find(entity);
            if (existing != null)
            {
                _unitOfWork.Context.Set<TEntity>().Remove(existing);
            }
        }

        public void Update(TEntity entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Modified;
            _unitOfWork.Context.Set<TEntity>().Attach(entity);
        }
    }
}
