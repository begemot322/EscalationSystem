using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EscalationService.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id); 

        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(c => c.UserId)
            .IsRequired(); 
        
        builder.HasOne<Escalation>()
            .WithMany()
            .HasForeignKey(c => c.EscalationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}