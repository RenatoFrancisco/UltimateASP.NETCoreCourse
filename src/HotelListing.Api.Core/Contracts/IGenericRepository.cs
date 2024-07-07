using HotelListing.Api.Core.Models;
using HotelListing.Api.Data;

namespace HotelListing.Api.Core.Contracts;

public interface IGenericRepository<T> where T : Entity, new()
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<TResult>> GetAllAsync<TResult>();
    Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
    Task<T> GetAsync(int? id);
    Task<TResult> GetAsync<TResult>(int? id);
    Task<bool> Exists(int id);
    Task<T> Addsync(T entity);
    Task<TResult> Addsync<TSource, TResult>(TSource source);
    Task UpdateAsync(T entity);
    Task UpdateAsync<TSource>(int? id, TSource source);
    Task DeleteAsync(int id);
}
