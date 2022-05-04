using AlertsApi.Domain.Entities;
using AlertsApi.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Db;

public class AlertDbContext : DbContext
{
    public virtual DbSet<Alert>? Alerts { get; set; }
    public virtual DbSet<DbMessage>? Message { get; set; }
    public virtual DbSet<Subscription>? Subscriptions { get; set; }

    public AlertDbContext(DbContextOptions<AlertDbContext> options) : base(options)
    {
        // this.ChangeTracker.AutoDetectChangesEnabled = false;
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