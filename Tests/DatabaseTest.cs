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
}