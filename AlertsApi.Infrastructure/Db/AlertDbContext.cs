using AlertsApi.Domain.Entities;
using AlertsApi.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AlertsApi.Infrastructure.Db;

public class AlertDbContext : DbContext
{
    public virtual DbSet<Alert>? Alerts { get; set; }
    public virtual DbSet<MessageEntity> Message { get; set; }

    public AlertDbContext(DbContextOptions<AlertDbContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AlertsConfiguration());
        modelBuilder.ApplyConfiguration(new MessagesConfiguration());
        //modelBuilder.Entity<Alert>().HasData(new Alert()
        //    {Id = 1, LocationName = "Kyiv", Active = true, UpdateTime = DateTime.Now});
    }
}