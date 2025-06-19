using System.Text.Json;

namespace PlatformManager.OrgAliases;

public class OrgAliasManager
{
    private readonly string _aliasFilePath;
    private Dictionary<string, string> _aliases;

    public OrgAliasManager()
    {
        _aliasFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".transport-aliases.json");
        _aliases = LoadAliases();
    }

    public void AddAlias(string alias, string organizationName)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(organizationName))
        {
            throw new ArgumentException("Alias and organization name cannot be empty");
        }

        _aliases[alias.ToLower()] = organizationName;
        SaveAliases();
    }

    public string? GetOrganizationName(string alias)
    {
        return _aliases.TryGetValue(alias.ToLower(), out var orgName) ? orgName : null;
    }

    public void RemoveAlias(string alias)
    {
        if (_aliases.Remove(alias.ToLower()))
        {
            SaveAliases();
        }
    }

    public Dictionary<string, string> GetAllAliases()
    {
        return new Dictionary<string, string>(_aliases);
    }

    public void ListAliases()
    {
        if (_aliases.Count == 0)
        {
            Console.WriteLine("No organization aliases configured.");
            return;
        }

        Console.WriteLine("Organization Aliases:");
        Console.WriteLine("=====================");
        foreach (var alias in _aliases)
        {
            Console.WriteLine($"{alias.Key} -> {alias.Value}");
        }
    }

    private Dictionary<string, string> LoadAliases()
    {
        if (!File.Exists(_aliasFilePath))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            var json = File.ReadAllText(_aliasFilePath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private void SaveAliases()
    {
        try
        {
            var json = JsonSerializer.Serialize(_aliases, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_aliasFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not save aliases: {ex.Message}");
        }
    }
} 