namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tmp.Interface.Repository;
using Tmp.Model.DatabaseEntity;

public class DatabaseRepository(TmpContext context) : IDbRepository
{
    private readonly TmpContext _context = context;

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var item = await _context.Set<T>().FirstOrDefaultAsync(predicate);
        return item;
    }

    /// <inheritdoc/>
    public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        var items = await _context.Set<T>().Where(predicate).ToListAsync();
        return items;
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync<T>(T item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        await _context.AddAsync(item);
        int count = await _context.SaveChangesAsync();
        return count;
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync<T>(List<T> items)
    {
        items.ForEach(async item => await _context.AddAsync(item ?? throw new Exception()));
        int count = await _context.SaveChangesAsync();
        return count;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync<T>(List<string> ids) where T : EntityBase
    {
        foreach (var id in ids)
        {
            T? item = await _context.FindAsync<T>(id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.LastModifiedOn = DateTime.UtcNow;
                item.LastModifiedBy = "sys";
                _context.Update(item);
            }
        }

        int count = await _context.SaveChangesAsync();
        return count;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync<T>(T item) where T : EntityBase
    {
        _context.Update(item);
        int count = await _context.SaveChangesAsync();
        return count;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync<T>(List<T> items) where T : EntityBase
    {
        foreach (T item in items)
        {
            _context.Update(item);
        }

        int count = await _context.SaveChangesAsync();
        return count;
    }
}
