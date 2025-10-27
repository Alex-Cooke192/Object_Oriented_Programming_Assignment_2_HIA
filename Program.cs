using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up EF Core options
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseSqlite("Data Source=Object_Oriented_Programming_Assignment_2_HIA-HAI_29-add-c-database/Data/jetconfigs.db")
            .Options; // This file path is the area used to initialise the DB

        // Create DB context
        using var db = new JetDbContext(options);

        //Create database file if not present & create all tables based on classes
        db.Database.EnsureCreated();

        // Create repository
        var repo = new JsonConfigurationRepository(db, currentUserId: 1);

        // Example to return db via command line
        var layouts = await repo.LoadAllAsync();

        foreach (var kvp in layouts)
        {
            Console.WriteLine($"Layout: {kvp.Key}, Cells: {kvp.Value.Components}");
        }
    }
}
