using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class JsonConfigurationRepository
{
    private readonly JetDbContext _db;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonConfigurationRepository(JetDbContext db)
    {
        _db = db;
    }

    public string ExportLayoutToJson()
    {
        var layout = new JetLayout
        {
            Layout = _db.JetLayoutCells
                .AsNoTracking()
                .Select(cell => new LayoutCell
                {
                    X = cell.X,
                    Y = cell.Y,
                    ComponentId = cell.ComponentId
                })
                .ToList()
        };

        return JsonSerializer.Serialize(layout, _options);
    }

    public void ImportLayoutFromJson(string json)
    {
        var layout = JsonSerializer.Deserialize<JetLayout>(json, _options);
        if (layout == null) return;

        _db.JetLayoutCells.RemoveRange(_db.JetLayoutCells); // Clear existing layout
        _db.JetLayoutCells.AddRange(layout.Layout.Select(cell => new JetLayoutCellDB
        {
            X = cell.X,
            Y = cell.Y,
            ComponentId = cell.ComponentId
        }));

        _db.SaveChanges();
    }
}