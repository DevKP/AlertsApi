using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AlertDbContext _dbContext;

    public MessageRepository(AlertDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(DbMessage message)
    {
        await _dbContext.Message!.AddAsync(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task InsertRangeAsync(IEnumerable<DbMessage> messages)
    {
        await _dbContext.Message!.AddRangeAsync(messages);
        await _dbContext.SaveChangesAsync();
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