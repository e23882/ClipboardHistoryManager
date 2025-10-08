using ClipboardHistoryManager.Models;
using System;
using System.IO;
using System.Text.Json;

namespace ClipboardHistoryManager.Services
{
    public static class SettingsService
    {
        private static readonly string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly string _appFolder = Path.Combine(_appDataPath, "ClipboardHistoryManager");
        private static readonly string _settingsFilePath = Path.Combine(_appFolder, "settings.json");

        public static Settings Load()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return new Settings();
            }

            try
            {
                string json = File.ReadAllText(_settingsFilePath);
                return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            catch
            {
                return new Settings(); // Return default settings on error
            }
        }

        public static void Save(Settings settings)
        {
            try
            {
                Directory.CreateDirectory(_appFolder);
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, json);
            }
            catch
            {
                // Handle exceptions (e.g., log the error)
            }
        }
    }
}
