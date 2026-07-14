using System;
using System.Windows.Forms;
using BluetoothBatteryViewer.Models;
using BluetoothBatteryViewer.Services;

namespace BluetoothBatteryViewer.UI
{
    public sealed class HotkeyTextBox : TextBox
    {
        public event EventHandler<HotkeySetting> HotkeyChanged;
        public event EventHandler InvalidHotkeyAttempted;

        public HotkeySetting CurrentHotkey { get; private set; } = new HotkeySetting();

        public HotkeyTextBox()
        {
            ReadOnly = true;
            TabStop = true;
            ShortcutsEnabled = false;
            AccessibleName = "热键区";
            AccessibleDescription = "按下组合热键进行设置，按退格键清空。";
            Text = "无";
        }

        public void SetHotkey(HotkeySetting setting)
        {
            CurrentHotkey = new HotkeySetting
            {
                Key = setting.Key,
                Modifiers = setting.Modifiers,
            };
            Text = CurrentHotkey.DisplayText;
            Select(TextLength, 0);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var keyCode = keyData & Keys.KeyCode;
            var modifiers = GetModifiers(keyData);

            if (keyCode == Keys.Back)
            {
                SetHotkey(new HotkeySetting());
                HotkeyChanged?.Invoke(this, CurrentHotkey);
                return true;
            }

            if (modifiers == HotkeyModifiers.None || HotkeyValidator.IsSingleModifierKey(keyCode))
            {
                return true;
            }

            var setting = new HotkeySetting
            {
                Key = keyCode,
                Modifiers = modifiers,
            };

            if (!HotkeyValidator.IsValid(setting))
            {
                InvalidHotkeyAttempted?.Invoke(this, EventArgs.Empty);
                return true;
            }

            SetHotkey(setting);
            HotkeyChanged?.Invoke(this, CurrentHotkey);
            return true;
        }

        private static HotkeyModifiers GetModifiers(Keys keyData)
        {
            var result = HotkeyModifiers.None;
            if (keyData.HasFlag(Keys.Control)) result |= HotkeyModifiers.Control;
            if (keyData.HasFlag(Keys.Shift)) result |= HotkeyModifiers.Shift;
            if (keyData.HasFlag(Keys.Alt)) result |= HotkeyModifiers.Alt;
            if ((Control.ModifierKeys & Keys.LWin) == Keys.LWin || (Control.ModifierKeys & Keys.RWin) == Keys.RWin)
            {
                result |= HotkeyModifiers.Win;
            }

            return result;
        }
    }
}