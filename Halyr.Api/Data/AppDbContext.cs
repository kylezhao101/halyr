using Halyr.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Halyr.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<FeatureFlagEnvironment> FeatureFlagEnvironments => Set<FeatureFlagEnvironment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FeatureFlag>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Key).IsRequired();
            entity.Property(f => f.Name).IsRequired();
            entity.HasIndex(f => f.Key).IsUnique();
        });

        modelBuilder.Entity<FeatureFlagEnvironment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Environment).HasConversion<string>().IsRequired();

            entity.HasIndex(e => new { e.FeatureFlagId, e.Environment }).IsUnique();
        });
    }
}