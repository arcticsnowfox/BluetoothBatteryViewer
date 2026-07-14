using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace BluetoothBatteryViewer.Models
{
    [Flags]
    public enum HotkeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
    }

    public sealed class HotkeySetting
    {
        public Keys Key { get; set; } = Keys.None;

        public HotkeyModifiers Modifiers { get; set; } = HotkeyModifiers.None;

        [JsonIgnore]
        public string DisplayText => HotkeyFormatter.ToDisplayString(this);
    }

    public static class HotkeyFormatter
    {
        public static string ToDisplayString(HotkeySetting setting)
        {
            if (setting.Key == Keys.None || setting.Modifiers == HotkeyModifiers.None)
            {
                return "无";
            }

            var parts = new List<string>();
            if (setting.Modifiers.HasFlag(HotkeyModifiers.Control)) parts.Add("Ctrl");
            if (setting.Modifiers.HasFlag(HotkeyModifiers.Shift)) parts.Add("Shift");
            if (setting.Modifiers.HasFlag(HotkeyModifiers.Alt)) parts.Add("Alt");
            if (setting.Modifiers.HasFlag(HotkeyModifiers.Win)) parts.Add("Windows");
            parts.Add(NormalizeKeyName(setting.Key));
            return string.Join(" + ", parts);
        }

        public static string NormalizeKeyName(Keys key)
        {
            switch (key)
            {
                case Keys.Prior:
                    return "PageUp";
                case Keys.Next:
                    return "PageDown";
                case Keys.Return:
                    return "Enter";
                case Keys.Capital:
                    return "CapsLock";
                case Keys.Back:
                    return "Backspace";
                case Keys.Menu:
                    return "Alt";
                case Keys.ControlKey:
                    return "Ctrl";
                case Keys.ShiftKey:
                    return "Shift";
                default:
                    return key.ToString();
            }
        }
    }
}