using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BluetoothBatteryViewer.Models;
using BluetoothBatteryViewer.Services;
using BluetoothBatteryViewer.UI;

namespace BluetoothBatteryViewer
{
    public sealed class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly SettingsService _settingsService = new SettingsService();
        private readonly StartupService _startupService = new StartupService();
        private readonly SoundService _soundService = new SoundService();
        private readonly SpeechService _speechService = new SpeechService();
        private readonly BluetoothBatteryService _batteryService = new BluetoothBatteryService();
        private readonly MessageWindow _messageWindow;
        private readonly HotkeyManager _hotkeyManager;

        private AppSettings _settings;

        public TrayApplicationContext()
        {
            Directory.CreateDirectory(AppPaths.DataDirectory);

            _settings = _settingsService.Load();
            if (_settings.RunAtStartup != _startupService.IsEnabled())
            {
                _settings.RunAtStartup = _startupService.IsEnabled();
            }

            _messageWindow = new MessageWindow(this);
            _hotkeyManager = new HotkeyManager(_messageWindow.Handle);

            var menu = new ContextMenuStrip();
            menu.Items.Add("设置", null, (s, e) => ShowSettings());
            menu.Items.Add("关于", null, (s, e) => ShowAbout());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("退出", null, (s, e) => ExitApplication());

            _notifyIcon = new NotifyIcon
            {
                Text = "蓝牙电池查看器",
                Icon = SystemIcons.Information,
                ContextMenuStrip = menu,
                Visible = true,
            };

            _notifyIcon.MouseDoubleClick += (s, e) => ShowSettings();

            _hotkeyManager.Register(_settings.Hotkey);
            _soundService.PlayStartupSound();
        }

        internal void ProcessHotkeyMessage()
        {
            _ = AnnounceBatteryAsync();
        }

        private async Task AnnounceBatteryAsync()
        {
            var text = await _batteryService.BuildAnnouncementAsync();
            _speechService.Speak(text, _settings);
        }

        private void ShowSettings()
        {
            using (var form = new SettingsForm(_settings))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var newSettings = form.GetSettings();
                _settingsService.Save(newSettings);
                _startupService.SetRunAtStartup(newSettings.RunAtStartup);
                _settings = newSettings;

                if (_settings.HasHotkey && !_hotkeyManager.Register(_settings.Hotkey))
                {
                    MessageBox.Show(
                        "热键注册失败，可能已经被其他程序占用。",
                        "蓝牙电池查看器",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (!_settings.HasHotkey)
                {
                    _hotkeyManager.Unregister();
                }
            }
        }

        private static void ShowAbout()
        {
            MessageBox.Show(
                "蓝牙电池查看器\r\n\r\n用于读取已连接蓝牙设备的电池电量，通过 Tolk 读屏接口或系统 TTS 朗读。",
                "关于",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ExitApplication()
        {
            _hotkeyManager.Unregister();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _messageWindow.DestroyHandle();
            _speechService.Dispose();
            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hotkeyManager.Unregister();
                _notifyIcon.Dispose();
                _speechService.Dispose();
            }

            base.Dispose(disposing);
        }

        private sealed class MessageWindow : NativeWindow
        {
            private readonly TrayApplicationContext _owner;

            public MessageWindow(TrayApplicationContext owner)
            {
                _owner = owner;
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == BluetoothBatteryViewer.Interop.NativeMethods.WmHotkey)
                {
                    _owner.ProcessHotkeyMessage();
                }

                base.WndProc(ref m);
            }
        }
    }
}