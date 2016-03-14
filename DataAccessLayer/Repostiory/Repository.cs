using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccessLayer.Repostiory
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> dbSet;

        public Repository(DbSet<T> db)
        {
            dbSet = db;
        }

        public Repository(DbContext dataContext)
        {
            dbSet = dataContext.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        
        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);


            query = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
                return orderBy(query).ToList();
            return query.ToList();
        }

        public IQueryable<T> GetQuery(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);


            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
                return orderBy(query);
            return query;
        }

        public virtual T GetByID(int id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(ICollection<T> entity)
        {
            dbSet.AddRange(entity);
        }

        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(T entityToUpdate)
        {
            dbSet.AddOrUpdate(entityToUpdate);
        }

        public virtual void Delete(int id)
        {
            dbSet.Remove(dbSet.Find(id));
        }

        public virtual void Delete(ICollection<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}