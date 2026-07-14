using System;
using System.IO;

namespace BluetoothBatteryViewer.Services
{

    public static class AppPaths
    {
        public static string BaseDirectory => AppContext.BaseDirectory;

        public static string DataDirectory
        {
            get
            {
                var path = Path.Combine(BaseDirectory, "Bluetooth battery");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string SettingsFilePath => Path.Combine(DataDirectory, "settings.json");

        public static string StartupSoundPath => Path.Combine(DataDirectory, "start.wav");

        public static string ReadersDirectory => Path.Combine(BaseDirectory, "Readers");
    }

}