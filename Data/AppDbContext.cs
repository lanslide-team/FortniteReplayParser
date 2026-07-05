using Microsoft.EntityFrameworkCore;

namespace FortniteReplayParser.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FortniteReplay>(e =>
        {
            e.Property(p => p.Id).IsRequired().HasColumnType("char(36)");
        });

        modelBuilder.Entity<FortniteReplayResult>(e =>
        {
            e.Property(p => p.Id).IsRequired().HasColumnType("char(36)");
            e.Property(p => p.EpicId).HasMaxLength(64).HasColumnType("varchar(64)");

            e.HasOne(r => r.Replay)
             .WithMany(p => p.Results)
             .HasForeignKey(r => r.FortniteReplaysId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
