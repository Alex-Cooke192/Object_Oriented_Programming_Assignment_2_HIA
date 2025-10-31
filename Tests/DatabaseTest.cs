using Microsoft.EntityFrameworkCore;

public class DatabaseTester
{
    private readonly JetDbContext _db;

    public DatabaseTester(JetDbContext db)
    {
        _db = db;
    }

    public async Task RunTestsAsync()
    {
        Console.WriteLine("🔍 Testing database tables...\n");

        await TestTableAsync("JetConfigs", _db.JetConfigs.CountAsync());
        await TestTableAsync("InteriorComponents", _db.InteriorComponents.CountAsync());
        await TestTableAsync("Users", _db.Users.CountAsync());
        await TestTableAsync("SeatProperties", _db.SeatProperties.CountAsync());
        await TestTableAsync("ToiletProperties", _db.ToiletProperties.CountAsync());
        await TestTableAsync("StorageCabinetProperties", _db.StorageCabinetProperties.CountAsync());
        await TestTableAsync("TableProperties", _db.TableProperties.CountAsync());
        await TestTableAsync("LightingProperties", _db.LightingProperties.CountAsync());
        await TestTableAsync("ScreenProperties", _db.ScreenProperties.CountAsync());
        await TestTableAsync("KitchenProperties", _db.KitchenProperties.CountAsync());
        await TestTableAsync("EmergencyExitProperties", _db.EmergencyExitProperties.CountAsync());

        await CheckJetConfigUserLinksAsync();

        Console.WriteLine("\n✅ All table checks complete.");
    }

    private async Task TestTableAsync(string tableName, Task<int> countTask)
    {
        try
        {
            int count = await countTask;
            Console.WriteLine($"✔ {tableName}: {count} row(s)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ {tableName}: Error - {ex.Message}");
        }
    }
    private async Task CheckJetConfigUserLinksAsync()
    // Verifies every layout is owned by a valid user
    {
        Console.WriteLine("\n🔗 Checking JetConfigs → Users foreign key integrity...");

        var validUserIds = await _db.Users.Select(u => u.Id).ToListAsync();
        var configs = await _db.JetConfigs.ToListAsync();

        foreach (var config in configs)
        {
            if (!validUserIds.Contains(config.UserId))
            {
                Console.WriteLine($"❌ JetConfig '{config.Name}' has invalid UserId: {config.UserId}");
            }
        }

        Console.WriteLine("✔ JetConfig user link check complete.");
    }
}