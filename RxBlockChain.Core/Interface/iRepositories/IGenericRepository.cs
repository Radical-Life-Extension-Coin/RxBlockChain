using System.Linq.Expressions;

namespace RxBlockChain.Core.Interface.iRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
       
        Task AddAsync(T entity);
        void Update(T entity);
        void DeleteAsync(T entity);
        void DeleteAllAsync(List<T> entities);
        Task SaveChangesAsync();
        Task<T> FindSingleAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}
