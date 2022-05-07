using AlertsApi.Domain.Entities;
using AlertsApi.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Db;

public sealed class AlertDbContext : DbContext
{
    public DbSet<Alert>? Alerts { get; set; }
    public DbSet<DbMessage>? Message { get; set; }
    public DbSet<Subscription>? Subscriptions { get; set; }

    public AlertDbContext(DbContextOptions<AlertDbContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        //Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AlertsConfiguration());
        modelBuilder.ApplyConfiguration(new MessagesConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionsConfiguration());
    }
}