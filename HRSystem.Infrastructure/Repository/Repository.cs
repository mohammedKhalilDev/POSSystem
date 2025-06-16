using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using POSSystem.Core.Repository;
using POSSystem.Infrastructure.ApplicationContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Infrastructure.Repository
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly ILogger<Repository<TEntity, TKey>> _logger;

        public Repository(AppDbContext context, ILogger<Repository<TEntity, TKey>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
            _logger = logger;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var property = Expression.Property(parameter, "Id");
            var equals = Expression.Equal(
                property,
                Expression.Constant(id, typeof(TKey)));
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);

                //SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entity)
        {
            try
            {
                await _dbSet.AddRangeAsync(entity);

                //SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public virtual Task Update(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;

        }

        public virtual Task Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;

        }

        public virtual Task RemoveRange(IEnumerable<TEntity> entity)
        {
            _dbSet.RemoveRange(entity);
            return Task.CompletedTask;
        }

        public virtual Task SaveChangesAsync()
        {
            _context.SaveChanges();

            return Task.CompletedTask;
        }

    }

    public class Repository<TEntity> : Repository<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        public Repository(AppDbContext context, ILogger<Repository<TEntity, int>> logger)
            : base(context, logger)
        {
        }
    }
}
