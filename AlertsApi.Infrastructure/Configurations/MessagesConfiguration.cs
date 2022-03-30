using AlertsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertsApi.Infrastructure.Configurations;

public class MessagesConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder.HasKey(alert => alert.Id);
        builder.Property(alert => alert.Message).IsRequired().HasMaxLength(500);
        builder.Property(alert => alert.Date).IsRequired();
    }
}