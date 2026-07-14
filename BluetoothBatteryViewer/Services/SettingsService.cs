using System.IO;
using System.Text.Json;
using BluetoothBatteryViewer.Models;

namespace BluetoothBatteryViewer.Services
{

    public sealed class SettingsService
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(AppPaths.SettingsFilePath))
                {
                    return new AppSettings();
                }

                var json = File.ReadAllText(AppPaths.SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            Directory.CreateDirectory(AppPaths.DataDirectory);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(AppPaths.SettingsFilePath, json);
        }
    }

}