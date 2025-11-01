using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Models;

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

        await TestTableAsync("JetConfigurations", _db.JetConfigurations.CountAsync());
        await TestTableAsync("InteriorComponents", _db.InteriorComponents.CountAsync());
        await TestTableAsync("Users", _db.Users.CountAsync());

        // Component property tables
        await TestTableAsync("SeatProperties", _db.SeatProperties.CountAsync());
        await TestTableAsync("ToiletProperties", _db.ToiletProperties.CountAsync());
        await TestTableAsync("StorageCabinetProperties", _db.StorageCabinetProperties.CountAsync());
        await TestTableAsync("TableProperties", _db.TableProperties.CountAsync());
        await TestTableAsync("LightingProperties", _db.LightingProperties.CountAsync());
        await TestTableAsync("ScreenProperties", _db.ScreenProperties.CountAsync());
        await TestTableAsync("KitchenProperties", _db.KitchenProperties.CountAsync());
        await TestTableAsync("EmergencyExitProperties", _db.EmergencyExitProperties.CountAsync());

        await CheckJetConfigurationUserLinksAsync();

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

    private async Task CheckJetConfigurationUserLinksAsync()
    {
        Console.WriteLine("\n🔗 Checking JetConfigurations → Users foreign key integrity...");

        var validUserIds = await _db.Users.Select(u => u.Id).ToListAsync();
        var configs = await _db.JetConfigurations.ToListAsync();

        foreach (var config in configs)
        {
            if (!validUserIds.Contains(config.UserId))
            {
                Console.WriteLine($"❌ JetConfiguration '{config.Name}' has invalid UserId: {config.UserId}");
            }
        }

        Console.WriteLine("✔ JetConfiguration user link check complete.");
    }
}