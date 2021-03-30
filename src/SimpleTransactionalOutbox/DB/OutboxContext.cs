using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class OutboxContext : DbContext
    {
        public virtual DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;

        public virtual DbSet<ProcessingData> ProcessingData { get; set; } = default!;

        public OutboxContext(DbContextOptions<OutboxContext> contextOptions)
           : base(contextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSnakeCaseNamingConvention();

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<OutboxMessage>()
                .HasKey(o => o.MessageId);

            _ = modelBuilder.Entity<ProcessingData>()
                .HasKey(e => e.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
