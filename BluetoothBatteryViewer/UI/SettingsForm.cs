using System;
using System.Drawing;
using System.Windows.Forms;
using BluetoothBatteryViewer.Models;

namespace BluetoothBatteryViewer.UI
{

    public sealed class SettingsForm : Form
    {
        private readonly Label _hotkeyLabel;
        private readonly HotkeyTextBox _hotkeyTextBox;
        private readonly CheckBox _runAtStartupCheckBox;
        private readonly CheckBox _useScreenReaderCheckBox;
        private readonly Button _saveButton;
        private readonly Button _cancelButton;
        private readonly Label _hintLabel;

        public SettingsForm(AppSettings settings)
        {
            Text = "设置";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 220);
            AccessibleName = "设置";

            _hotkeyLabel = new Label
            {
                AutoSize = true,
                Location = new Point(20, 20),
                Text = "设定热键：",
                AccessibleName = "设定热键",
            };

            _hotkeyTextBox = new HotkeyTextBox
            {
                Location = new Point(20, 45),
                Size = new Size(370, 27),
            };

            _hintLabel = new Label
            {
                AutoSize = false,
                Location = new Point(20, 78),
                Size = new Size(370, 36),
                Text = "按下组合键即可设置热键。单键无效，按退格键清空。系统级热键不会生效。",
            };

            _runAtStartupCheckBox = new CheckBox
            {
                AutoSize = true,
                Location = new Point(20, 122),
                Text = "开机启动",
                AccessibleName = "开机启动",
            };

            _useScreenReaderCheckBox = new CheckBox
            {
                AutoSize = true,
                Location = new Point(20, 150),
                Text = "使用读屏朗读（Tolk）",
                AccessibleName = "使用读屏朗读",
            };

            _saveButton = new Button
            {
                Text = "保存",
                Location = new Point(220, 178),
                Size = new Size(80, 28),
                DialogResult = DialogResult.OK,
            };

            _cancelButton = new Button
            {
                Text = "取消",
                Location = new Point(310, 178),
                Size = new Size(80, 28),
                DialogResult = DialogResult.Cancel,
            };

            Controls.AddRange(new Control[]
            {
            _hotkeyLabel, _hotkeyTextBox, _hintLabel,
            _runAtStartupCheckBox, _useScreenReaderCheckBox,
            _saveButton, _cancelButton,
            });

            AcceptButton = _saveButton;
            CancelButton = _cancelButton;

            _hotkeyTextBox.InvalidHotkeyAttempted += (s, e) => System.Media.SystemSounds.Exclamation.Play();
            _hotkeyTextBox.SetHotkey(settings.Hotkey);
            _runAtStartupCheckBox.Checked = settings.RunAtStartup;
            _useScreenReaderCheckBox.Checked = settings.UseScreenReaderSpeech;

            Shown += (s, e) => _hotkeyTextBox.Focus();
        }

        public AppSettings GetSettings()
        {
            return new AppSettings
            {
                Hotkey = new HotkeySetting
                {
                    Key = _hotkeyTextBox.CurrentHotkey.Key,
                    Modifiers = _hotkeyTextBox.CurrentHotkey.Modifiers,
                },
                RunAtStartup = _runAtStartupCheckBox.Checked,
                UseScreenReaderSpeech = _useScreenReaderCheckBox.Checked,
            };
        }
    }

}