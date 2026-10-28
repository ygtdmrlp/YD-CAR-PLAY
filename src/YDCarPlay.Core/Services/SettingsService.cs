using System;
using System.IO;
using System.Text.Json;
using YDCarPlay.Core.Interfaces;
using YDCarPlay.Core.Models;

namespace YDCarPlay.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _settings;

        public AppSettings Settings => _settings;
        public event EventHandler<AppSettings>? SettingsChanged;

        public SettingsService(string? customPath = null)
        {
            if (!string.IsNullOrWhiteSpace(customPath))
            {
                _settingsFilePath = customPath;
            }
            else
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dir = Path.Combine(appData, "YDCarPlay");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                _settingsFilePath = Path.Combine(dir, "settings.json");
            }

            _settings = new AppSettings();
            Load();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (loaded != null)
                    {
                        _settings = loaded;
                        SettingsChanged?.Invoke(this, _settings);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SettingsService] Load error: {ex.Message}");
            }

            // Defaults if file doesn't exist or failed to load
            _settings = new AppSettings();
            Save();
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_settingsFilePath, json);

                SettingsChanged?.Invoke(this, _settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SettingsService] Save error: {ex.Message}");
            }
        }

        public void ResetToDefaults()
        {
            _settings = new AppSettings();
            Save();
        }
    }
}
