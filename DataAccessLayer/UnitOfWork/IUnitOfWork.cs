using System;
using DataAccessLayer.Repostiory;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        void Save();      
    }
}
