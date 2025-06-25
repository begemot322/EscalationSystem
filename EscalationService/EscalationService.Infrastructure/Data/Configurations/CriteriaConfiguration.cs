using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class CriteriaConfiguration : IEntityTypeConfiguration<Criteria>
{
    public void Configure(EntityTypeBuilder<Criteria> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(700);
        
        builder.Property(c => c.Order)
            .IsRequired();
        
        builder.Property(c => c.IsCompleted)
            .IsRequired();
        
        builder.HasOne<Escalation>()
            .WithMany() 
            .HasForeignKey(c => c.EscalationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}