using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using AlertsApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AlertDbContext _dbContext;

    public MessageRepository(AlertDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(DbMessage message)
    {
        await _dbContext.Message!.AddAsync(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddOrUpdateAsync(DbMessage message)
    {
        var exists = await _dbContext.Message!.AsNoTracking().AnyAsync(m => m.Id == message.Id);
        if (exists)
        {
            _dbContext.Message!.Update(message);
        }
        else
        {
            await _dbContext.Message!.AddAsync(message);
        }

        await _dbContext.SaveChangesAsync();
        _dbContext.DetachEntry(message);
    }

    public async Task AddOrUpdateRangeAsync(IEnumerable<DbMessage> messages)
    {
        var messagesList = messages.ToList();
        _dbContext.Message!.UpdateRange(messagesList);
        await _dbContext.SaveChangesAsync();
        //_dbContext.DetachEntries(messagesList);
    }

    public async Task<IEnumerable<DbMessage>> GetAllAsync()
    {
        return await _dbContext.Message!
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DbMessage?> GetById(int id)
    {
        return await _dbContext.Message!
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<DbMessage?> GetNewestAsync()
    {
        return await _dbContext.Message!
            .AsNoTracking()
            .OrderByDescending(m => m.Id)
            .FirstOrDefaultAsync();
    }
}