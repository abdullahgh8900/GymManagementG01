using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Repositories.Implementaion;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork 
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new();
        public ISessionRepository SessionRepository { get; }
        public UnitOfWork(GymDbContext dbContext , ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
        }


        IGenericRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out var repository))
                return (IGenericRepository<TEntity>)repository;

            var newRepo = new GenericRepository<TEntity>(_dbContext);
            _repositories [entityType] = newRepo;
            return newRepo;

        }

        int IUnitOfWork.SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
