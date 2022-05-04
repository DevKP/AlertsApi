using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Extensions;

public static class DbContextExtensions
{
    public static void DetachEntry<TEntity>(this DbContext context, TEntity entity) where TEntity : class
    {
        context.Entry<TEntity>(entity).State = EntityState.Detached;
    }
}