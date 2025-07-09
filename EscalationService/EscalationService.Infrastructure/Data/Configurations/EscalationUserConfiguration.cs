using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class EscalationUserConfiguration : IEntityTypeConfiguration<EscalationUser>
{
    public void Configure(EntityTypeBuilder<EscalationUser> builder)
    {
        builder.HasKey(eu => new { eu.EscalationId, eu.UserId });
        
        builder.Property(eu => eu.EscalationId).IsRequired();
        builder.Property(eu => eu.UserId).IsRequired();
        
        builder.HasOne(eu => eu.Escalation)
            .WithMany(e => e.EscalationUsers)
            .HasForeignKey(eu => eu.EscalationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}