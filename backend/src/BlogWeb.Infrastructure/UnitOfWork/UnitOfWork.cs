using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Application.Interfaces.Repository;
using BlogWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BlogWeb.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
         private IDbContextTransaction _transaction;
        private bool _disposed;

        private readonly ILogger _logger;
        private IUserRepository _userRepository;
        public IUserRepository UserRepository
        {
            get { return _userRepository = _userRepository ?? new UserRepository(_dbContext,_logger); }
        }

        public UnitOfWork(ApplicationDbContext dbContext, IUserRepository userRepository, ILogger logger, IDbContextTransaction transaction)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _logger = logger;
            _transaction = transaction;
        }
        public void Commit()
            => _dbContext.SaveChanges();


        public async Task CommitAsync()
            => await _dbContext.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _dbContext.Dispose();
            _disposed = true;
        }

        public void Rollback()
            => _dbContext.Dispose();


        public async Task RollbackAsync()
            => await _dbContext.DisposeAsync();

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
             _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }
    }
}