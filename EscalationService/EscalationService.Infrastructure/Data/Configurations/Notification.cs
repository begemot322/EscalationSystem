using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.IsRead)
            .IsRequired();
        
        // Связь с Escalation (опционально)
        builder.HasOne(n => n.Escalation)
            .WithMany() // ← если в Escalation не будет ICollection<Notification>
            .HasForeignKey(n => n.EscalationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Связь с NotificationChannel (обязательно)
        builder.HasOne(n => n.NotificationChannel)
            .WithMany(nc => nc.Notifications)
            .HasForeignKey(n => n.NotificationChannelId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasData(
            new Notification
            {
                Id = 1,
                Title = "Новая эскалация",
                Message = "Вам назначена новая эскалация #1",
                CreatedAt = new DateTime(2024, 5, 5, 10, 0, 0, DateTimeKind.Utc),
                IsRead = false,
                UserId = 1,
                EscalationId = 1,
                NotificationChannelId = 1
            },
            new Notification
            {
                Id = 2,
                Title = "Статус изменён",
                Message = "Эскалация #1 переведена в статус 'В работе'",
                CreatedAt = new DateTime(2023, 5, 5, 10, 0, 0, DateTimeKind.Utc),
                IsRead = true,
                UserId = 1,
                EscalationId = 1,
                NotificationChannelId = 2
            }
        );
    }
}