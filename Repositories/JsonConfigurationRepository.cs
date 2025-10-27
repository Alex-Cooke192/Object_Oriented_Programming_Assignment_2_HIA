using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonConfigurationRepository
{
    private const string FilePath = "Data/jet_layout.json";

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JetLayout LoadLayout()
    {
        if (!File.Exists(FilePath))
            return new JetLayout(); // Return empty layout if file doesn't exist

        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<JetLayout>(json, _options) ?? new JetLayout();
    }

    public void SaveLayout(JetLayout layout)
    {
        Directory.CreateDirectory("Data");
        string json = JsonSerializer.Serialize(layout, _options);
        File.WriteAllText(FilePath, json);
    }
}