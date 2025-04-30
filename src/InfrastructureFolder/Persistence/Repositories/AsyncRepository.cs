using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using System.Linq.Expressions;
using Persistence.Confgurations;

namespace Persistence.Repositories
{
    public class AsyncRepository<TEntity> : IAsyncRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;
        protected DataContext Context;

        public AsyncRepository(DataContext context)
        {
            Context = context;
            _dbSet = Context.Set<TEntity>();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public Task<TEntity> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IQueryable<TEntity>> WhereQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }
    }
}
