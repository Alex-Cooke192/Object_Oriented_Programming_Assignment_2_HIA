using Microsoft.EntityFrameworkCore;

namespace JIDS.Data
{
    public class JetDbContext : DbContext
    {
        public JetDbContext(DbContextOptions<JetDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<UserDB> Users { get; set; }
        public DbSet<JetConfigurationDB> JetConfigurations { get; set; }
        public DbSet<InteriorComponentDB> InteriorComponents { get; set; }
        public DbSet<ComponentSettings> ComponentSettings { get; set; }

        // Model configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users -> JetConfiguration (one-to-many)
            modelBuilder.Entity<UserDB>()
                .HasMany(u => u.Configurations)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade); 

            // JetConfiguration → InteriorComponents (one-to-many)
            modelBuilder.Entity<JetConfigurationDB>()
                .HasMany(c => c.InteriorComponents)
                .WithOne()
                .HasForeignKey(ic => ic.ConfigID)
                .OnDelete(DeleteBehavior.Cascade);

            // InteriorComponent → ComponentSettings (one-to-one)
            modelBuilder.Entity<InteriorComponentDB>()
                .HasOne(ic => ic.InteriorComponentSettings)
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
            modelBuilder.Entity<InteriorComponentDB>()
                .Property(ic => ic.Position)
                .HasColumnType("TEXT");

            modelBuilder.Entity<ComponentSettings>()
                .Property(cs => cs.SettingsJson)
                .HasColumnType("TEXT");

        }
    }
}