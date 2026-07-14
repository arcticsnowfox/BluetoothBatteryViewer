using System; // 补充：AppContext 需要此命名空间
using System.IO;
using Microsoft.Win32;

namespace BluetoothBatteryViewer.Services
{
    public sealed class StartupService
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "BluetoothBatteryViewer";

        public void SetRunAtStartup(bool enabled)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true) ?? Registry.CurrentUser.CreateSubKey(RunKey))
            {
                if (enabled)
                {
                    key.SetValue(ValueName, Quote(Path.Combine(AppContext.BaseDirectory, "BluetoothBatteryViewer.exe")));
                }
                else
                {
                    key.DeleteValue(ValueName, false);
                }
            }
        }

        public bool IsEnabled()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false))
            {
                return key?.GetValue(ValueName) is string value && !string.IsNullOrWhiteSpace(value);
            }
        }

        private static string Quote(string path) => $"\"{path}\"";
    }
}