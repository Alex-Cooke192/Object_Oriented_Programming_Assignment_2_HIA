using Microsoft.EntityFrameworkCore;

public class JetDbContext : DbContext
{
    public JetDbContext(DbContextOptions<JetDbContext> options) : base(options) { }
    public DbSet<UserDB> Users { get; set; }
    public DbSet<JetConfigDB> JetConfigs { get; set; }
    public DbSet<InteriorComponentDB> InteriorComponents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=Data/jet_interior.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // USER → JETCONFIGURATION (many-to-one)
        modelBuilder.Entity<JetConfigDB>()
            .HasOne(cfg => cfg.User)
            .WithMany(u => u.JetConfigs)
            .HasForeignKey(cfg => cfg.UserId);

        /*
        // JETCONFIGURATION → INTERIOR COMPONENT
        modelBuilder.Entity<InteriorComponentDB>()
            .HasOne(comp => comp.Config)
            .WithMany(cfg => cfg.InteriorComponents)
            .HasForeignKey(comp => comp.ConfigID);
            */
        
    //Base table for inheritance
    modelBuilder.Entity<InteriorComponentDB>().ToTable("InteriorComponents");

    // Subtype tables
    modelBuilder.Entity<SeatComponentDB>().ToTable("SeatComponents");
    modelBuilder.Entity<KitchenComponentDB>().ToTable("KitchenComponents");
    modelBuilder.Entity<LightingComponentDB>().ToTable("LightingComponents");
    modelBuilder.Entity<TableComponentDB>().ToTable("TableComponents");
    modelBuilder.Entity<ScreenComponentDB>().ToTable("ScreenComponents");
    modelBuilder.Entity<StorageCabinetComponentDB>().ToTable("StorageCabinetComponents");
    modelBuilder.Entity<EmergencyExitComponentDB>().ToTable("EmergencyExitComponents");    
    }
}