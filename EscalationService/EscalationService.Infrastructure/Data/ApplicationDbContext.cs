using System.Reflection;
using EscalationService.Domain;
using EscalationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EscalationService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Criteria> Criterias { get; set; }
    public DbSet<Escalation> Escalations { get; set; }
    public DbSet<EscalationUser> EscalationUsers { get; set; }

    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AttachmentType> AttachmentTypes { get; set; }
    public DbSet<NotificationChannel> NotificationChannels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

}