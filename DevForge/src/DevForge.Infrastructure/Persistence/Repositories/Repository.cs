using DevForge.Domain.Common;
using DevForge.Domain.Specifications;
using DevForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevForge.Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(specification.ToExpression()).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> FindOneAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(specification.ToExpression(), cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(specification.ToExpression(), cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(specification.ToExpression(), cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
