using System;
using System.Windows.Forms;
using BluetoothBatteryViewer.Interop;
using BluetoothBatteryViewer.Models;

namespace BluetoothBatteryViewer.Services
{

    public sealed class HotkeyManager
    {
        private readonly IntPtr _handle;
        private readonly int _hotkeyId;
        private bool _registered;

        public HotkeyManager(IntPtr handle, int hotkeyId = 0xB710)
        {
            _handle = handle;
            _hotkeyId = hotkeyId;
        }

        public bool Register(HotkeySetting setting)
        {
            Unregister();

            if (!HotkeyValidator.IsValid(setting))
            {
                return false;
            }

            _registered = NativeMethods.RegisterHotKey(
                _handle,
                _hotkeyId,
                ToNativeModifiers(setting.Modifiers) | NativeMethods.ModNoRepeat,
                (uint)setting.Key);

            return _registered;
        }

        public void Unregister()
        {
            if (!_registered)
            {
                return;
            }

            NativeMethods.UnregisterHotKey(_handle, _hotkeyId);
            _registered = false;
        }

        public bool Matches(Message message)
        {
            return message.Msg == NativeMethods.WmHotkey && message.WParam == (IntPtr)_hotkeyId;
        }

        private static uint ToNativeModifiers(HotkeyModifiers modifiers)
        {
            uint result = 0;
            if (modifiers.HasFlag(HotkeyModifiers.Alt)) result |= NativeMethods.ModAlt;
            if (modifiers.HasFlag(HotkeyModifiers.Control)) result |= NativeMethods.ModControl;
            if (modifiers.HasFlag(HotkeyModifiers.Shift)) result |= NativeMethods.ModShift;
            if (modifiers.HasFlag(HotkeyModifiers.Win)) result |= NativeMethods.ModWin;
            return result;
        }
    }

}