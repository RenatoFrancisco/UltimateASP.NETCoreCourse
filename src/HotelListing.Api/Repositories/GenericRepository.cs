using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : Entity, new()
{
    private readonly HotelListingDbContext _context;

    protected DbSet<T> DbSet { get; }

    public GenericRepository(HotelListingDbContext context)
    {
        DbSet = context.Set<T>();
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<T> GetAsync(int id) => await DbSet.FindAsync(id);

    public async Task<bool> Exists(int id) => await DbSet.FindAsync(id) is not null;

    public async Task<T> Addsync(T entity)
    {
        DbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        DbSet.Remove(new T { Id = id });
        await _context.SaveChangesAsync();
    }
}
