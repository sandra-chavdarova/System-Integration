using System.Linq.Expressions;
using HotelApplication.Domain.Common;
using HotelApplication.Domain.Dto;
using Microsoft.EntityFrameworkCore.Query;

namespace HotelApplication.Repository.Interface;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> InsertAsync(T entity);
    Task<ICollection<T>> InsertManyAsync(ICollection<T> entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
    Task<E?> Get<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<IEnumerable<E>> GetAllAsync<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    
    public Task<PaginatedResult<E>> GetAllPagedAsync<E>(
        Expression<Func<T, E>> selector,
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = false);
}
