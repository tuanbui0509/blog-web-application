using System.Linq.Expressions;
using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogWeb.Infrastructure.UnitOfWork
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        public readonly ILogger _logger;
        private readonly DbSet<T> _entitySet;

        public Repository(ILogger logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _entitySet = _dbContext.Set<T>();
        }

        public void Add(T entity) => _dbContext.Add(entity);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await _dbContext.AddAsync(entity);

        public void AddRange(IEnumerable<T> entities) => _dbContext.AddRange(entities);

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) => await _dbContext.AddRangeAsync(entities);

        public T Get(Expression<Func<T, bool>> expression) => _entitySet.FirstOrDefault(expression);

        public IEnumerable<T> GetAll() => _entitySet.AsEnumerable();

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> expression) => _entitySet.Where(expression).AsEnumerable();

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) => await _entitySet.ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) => await _entitySet.Where(expression).ToListAsync(cancellationToken);

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) => await _entitySet.FirstOrDefaultAsync(expression);

        public void Remove(T entity) => _dbContext.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbContext.RemoveRange(entities);

        public void Update(T entity) => _dbContext.Update(entity);

        public void UpdateRange(IEnumerable<T> entities) => _dbContext.UpdateRange(entities);
    }
}