using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Core.Repositories;

public class GenericRepository<T>(HotelListingDbContext context, IMapper mapper) : IGenericRepository<T> where T : Entity, new()
{
    protected DbSet<T> DbSet { get; } = context.Set<T>();


    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
    {
        var totalSize = await context.Set<T>().CountAsync();
        var items = await DbSet
            .Skip(queryParameters.StartIndex)
            .Take(queryParameters.PageSize)
            .ProjectTo<TResult>(mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<TResult>
        {
            Items = items,
            PageNumber = queryParameters.PageNumber,
            RecordNumber = queryParameters.PageSize,
            TotalCount = totalSize
        };
    }

    public async Task<T> GetAsync(int id) => await DbSet.FindAsync(id);

    public async Task<bool> Exists(int id) => await DbSet.AnyAsync(x => x.Id == id);

    public async Task<T> Addsync(T entity)
    {
        DbSet.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }
    public async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        DbSet.Remove(new T { Id = id });
        await context.SaveChangesAsync();
    }
}
