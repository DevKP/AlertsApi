using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Extensions;

public static class DbContextExtensions
{
    public static void DetachEntry<TEntity>(this DbContext context, TEntity entity) where TEntity : class
    {
        context.Entry<TEntity>(entity).State = EntityState.Detached;
    }

    public static void DetachEntries<TEntity>(this DbContext context, IEnumerable<TEntity> entities) where TEntity : class
    {
        foreach (var entity in entities)
        {
            context.Entry<TEntity>(entity).State = EntityState.Detached;
        }
    }

    public static void DetachAllEntries(this DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            context.Entry(entry.Entity).State = EntityState.Detached;
        }
    }
}