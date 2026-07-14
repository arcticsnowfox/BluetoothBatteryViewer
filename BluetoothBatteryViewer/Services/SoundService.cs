using System.IO;
using System.Media;

namespace BluetoothBatteryViewer.Services
{
    public sealed class SoundService
    {
        public void PlayStartupSound()
        {
            try
            {
                if (File.Exists(AppPaths.StartupSoundPath))
                {
                    using (var player = new SoundPlayer(AppPaths.StartupSoundPath))
                    {
                        player.Play();
                    }
                }
            }
            catch
            {
            }
        }
    }
}