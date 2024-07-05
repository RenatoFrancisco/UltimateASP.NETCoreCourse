using HotelListing.Api.Data;
using HotelListing.Api.Core.Models;

namespace HotelListing.Api.Core.Contracts;

public interface IGenericRepository<T> where T : Entity, new()
{
    Task<T> GetAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
    Task<T> Addsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> Exists(int id);
}
