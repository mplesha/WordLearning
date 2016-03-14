using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccessLayer.Repostiory
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

 

        T GetByID(int objID);

        void Insert(ICollection<T> obj);

        void Insert(T obj);

        void Update(T obj);

        void Delete(int objID);

        void Delete(ICollection<T> obj);

        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        IQueryable<T> GetQuery(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
    }
}
