using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text;
using DataAccessLayer.Entities;
using DataAccessLayer.Repostiory;

namespace DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public FinalWordLearn Context { get; set; }
        private Dictionary<Type, object> repositories;
        private bool disposed;

        public UnitOfWork()
        {
            Context = new FinalWordLearn();
            repositories = new Dictionary<Type, object>();
        }

        public UnitOfWork(FinalWordLearn context)
        {
            Context = context;
            repositories = new Dictionary<Type, object>();
        }

       

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories.ContainsKey(typeof (TEntity)))
                return repositories[typeof (TEntity)] as IRepository<TEntity>;

            var repository = new Repository<TEntity>(Context);
            repositories.Add(typeof (TEntity), repository);
            return repository;
        }

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(
                        string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name,
                            eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName,
                            ve.ErrorMessage));
                    }
                }
                throw new DbEntityValidationException(sb.ToString(), e);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                    Context.Dispose();

            disposed = true;
        }
    }
}
