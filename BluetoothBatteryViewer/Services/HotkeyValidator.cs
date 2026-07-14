using BluetoothBatteryViewer.Models;
using System.Windows.Forms;

namespace BluetoothBatteryViewer.Services
{
    public static class HotkeyValidator
    {
        public static bool IsValid(HotkeySetting setting)
        {
            if (setting.Key == Keys.None || setting.Modifiers == HotkeyModifiers.None)
            {
                return false;
            }

            if (IsSingleModifierKey(setting.Key))
            {
                return false;
            }

            var key = setting.Key;
            var modifiers = setting.Modifiers;

            if (key == Keys.Tab && modifiers != HotkeyModifiers.None) return false;
            if (key == Keys.Space && (modifiers == HotkeyModifiers.Control || modifiers == HotkeyModifiers.Win)) return false;
            if (key == Keys.Return && modifiers == HotkeyModifiers.Alt) return false;
            if (key == Keys.Delete && modifiers.HasFlag(HotkeyModifiers.Control) && modifiers.HasFlag(HotkeyModifiers.Alt)) return false;
            if (key == Keys.ShiftKey && modifiers == HotkeyModifiers.Control) return false;
            if (key == Keys.ShiftKey && modifiers == HotkeyModifiers.Alt) return false;

            return true;
        }

        public static bool IsSingleModifierKey(Keys key)
        {
            // 修正：将 C# 9.0 的 is ... or 替换为传统的逻辑或判断
            return key == Keys.ControlKey ||
                   key == Keys.ShiftKey ||
                   key == Keys.Menu ||
                   key == Keys.LWin ||
                   key == Keys.RWin;
        }
    }
}