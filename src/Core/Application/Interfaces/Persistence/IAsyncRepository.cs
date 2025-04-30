using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(string id);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> DeleteAsync(string id);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IQueryable<TEntity>> WhereQueryable(Expression<Func<TEntity, bool>> predicate);
    }
}
