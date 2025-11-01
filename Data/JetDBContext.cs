using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Data
{
    public class JetDbContext : DbContext
    {
        public JetDbContext(DbContextOptions<JetDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<JetConfiguration> JetConfigurations { get; set; }
        public DbSet<InteriorComponent> InteriorComponents { get; set; }
        public DbSet<ComponentSettings> ComponentSettings { get; set; }

        // Model configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JetConfiguration → InteriorComponents (one-to-many)
            modelBuilder.Entity<JetConfiguration>()
                .HasMany(c => c.InteriorComponents)
                .WithOne()
                .HasForeignKey(ic => ic.ConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            // InteriorComponent → ComponentSettings (one-to-one)
            modelBuilder.Entity<InteriorComponent>()
                .HasOne(ic => ic.ComponentSettings)
                .WithOne()
                .HasForeignKey<ComponentSettings>(cs => cs.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);

            //
            // Property configuration
            //
            // The Position property will contain serialized {x, y} data.
            // The SettingsJson property holds all type-specific settings in JSON.
            // Adjust column types for your database provider:
            // - SQL Server → nvarchar(max)
            // - PostgreSQL → jsonb
            modelBuilder.Entity<InteriorComponent>()
                .Property(ic => ic.Position)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<ComponentSettings>()
                .Property(cs => cs.SettingsJson)
                .HasColumnType("nvarchar(max)");

        }
    }
}