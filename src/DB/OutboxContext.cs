using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

namespace DB;

/// <summary>
/// Database context.
/// </summary>
public class OutboxContext : DbContext
{
    /// <summary>
    /// Outbox messages table.
    /// </summary>
    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;

    /// <summary>
    /// Processing data table.
    /// </summary>
    public virtual DbSet<ProcessingData> ProcessingData { get; set; } = default!;

    /// <summary>
    /// Creates <see cref="OutboxContext"/> instance.
    /// </summary>
    /// <param name="contextOptions">Options for context.</param>
    public OutboxContext(DbContextOptions<OutboxContext> contextOptions)
       : base(contextOptions)
    {
        _ = Database.EnsureCreated();
    }

    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<OutboxMessage>()
            .HasKey(o => o.MessageId);

        _ = modelBuilder.Entity<ProcessingData>()
            .HasKey(e => e.Id);

        base.OnModelCreating(modelBuilder);
    }
}
