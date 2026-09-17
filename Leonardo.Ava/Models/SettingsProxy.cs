using Leonardo.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Leonardo.Properties;

public class SettingsProxy : ILeonardoSettings
{
    private readonly string _settingsPath;

    public SettingsProxy()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataFolder, "Leonardo");

        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "settings.json");
    }

    public string this[ELSetting key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public string Get(ELSetting key)
    {
        var settings = Load();
        return settings.TryGetValue(key.ToString(), out var value) && !string.IsNullOrEmpty(value)
            ? value
            : string.Empty;
    }

    private void Set(ELSetting key, string value)
    {
        var settings = Load();
        settings[key.ToString()] = value ?? string.Empty;
        Save(settings);
    }

    private Dictionary<string, string> Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }
        catch (IOException)
        {
            return new Dictionary<string, string>();
        }
        catch (UnauthorizedAccessException)
        {
            return new Dictionary<string, string>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, string>();
        }
    }

    private void Save(Dictionary<string, string> settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_settingsPath, json);
    }
}
