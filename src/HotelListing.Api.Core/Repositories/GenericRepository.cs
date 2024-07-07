using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Core.Exceptions;
using HotelListing.Api.Core.Models;
using HotelListing.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Core.Repositories;

public class GenericRepository<T>(HotelListingDbContext context, IMapper mapper) : IGenericRepository<T> where T : Entity, new()
{
    protected DbSet<T> DbSet { get; } = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>() =>
        await context.Set<T>()
            .ProjectTo<TResult>(mapper.ConfigurationProvider)
            .ToListAsync();

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

    public async Task<T> GetAsync(int? id) => await DbSet.FindAsync(id);

    public async Task<TResult> GetAsync<TResult>(int? id)
    {
        if (id is null)
            throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No key provided");

        var result = await DbSet.FindAsync(id);
        return mapper.Map<TResult>(result);

    }

    public async Task<TResult> Addsync<TSource, TResult>(TSource source)
    {
        var entity = mapper.Map<T>(source);
        await context.AddAsync(entity);
        await context.SaveChangesAsync();

        return mapper.Map<TResult>(entity);
    }

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

    public async Task UpdateAsync<TSource>(int? id, TSource source)
    {
        var entity = await GetAsync(id);
        if (entity is null)
            throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No key provided");

        mapper.Map(source, entity);
        context.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        DbSet.Remove(new T { Id = id });
        await context.SaveChangesAsync();
    }
}
