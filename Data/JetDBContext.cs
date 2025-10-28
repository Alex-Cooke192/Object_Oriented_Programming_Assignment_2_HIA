using Microsoft.EntityFrameworkCore;

public class JetDbContext : DbContext
{
    public JetDbContext(DbContextOptions<JetDbContext> options) : base(options) { }
    public DbSet<UserDB> Users { get; set; }
    public DbSet<JetConfigDB> JetConfigs { get; set; }
    public DbSet<InteriorComponentDB> InteriorComponents { get; set; }

    public DbSet<SeatPropertiesDB> SeatProperties { get; set; }
    public DbSet<ToiletPropertiesDB> ToiletProperties { get; set; }
    public DbSet<TablePropertiesDB> TableProperties { get; set; }
    public DbSet<ScreenPropertiesDB> ScreenProperties { get; set; }
    public DbSet<LightingPropertiesDB> LightingProperties { get; set; }
    public DbSet<EmergencyExitPropertiesDB> EmergencyExitProperties { get; set; }
    public DbSet<KitchenPropertiesDB> KitchenProperties { get; set; }
    public DbSet<StorageCabinetPropertiesDB> StorageCabinetProperties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=Data/jet_interior.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User → JetConfigs (many-to-one)
        modelBuilder.Entity<JetConfigDB>()
            .HasOne(j => j.User)
            .WithMany(u => u.JetConfigs)
            .HasForeignKey(j => j.UserId);

        // Component → Properties (one-to-one or one-to-many depending on design)
        modelBuilder.Entity<SeatPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<ToiletPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<TablePropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<ScreenPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<LightingPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<EmergencyExitPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<KitchenPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);

        modelBuilder.Entity<StorageCabinetPropertiesDB>()
            .HasOne(p => p.Component)
            .WithMany()
            .HasForeignKey(p => p.ComponentId);
    }
}