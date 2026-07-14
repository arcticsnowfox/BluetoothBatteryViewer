using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using BluetoothBatteryViewer.Models;

namespace BluetoothBatteryViewer.Services
{
    public sealed class SpeechService : IDisposable
    {
        private readonly SpeechSynthesizer _fallbackSynth = new SpeechSynthesizer();
        private bool _tolkLoaded;

        public void Speak(string text, AppSettings settings)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (settings.UseScreenReaderSpeech && TrySpeakWithTolk(text))
            {
                return;
            }

            _fallbackSynth.SpeakAsyncCancelAll();
            _fallbackSynth.SpeakAsync(text);
        }

        private bool TrySpeakWithTolk(string text)
        {
            try
            {
                var dllPath = ResolveLibraryPath("Tolk.dll");
                if (dllPath == null || !EnsureLoaded(dllPath))
                {
                    return false;
                }

                if (!_tolkLoaded)
                {
                    if (!TolkNative.Tolk_Load())
                    {
                        return false;
                    }

                    _tolkLoaded = true;
                }

                return TolkNative.Tolk_Output(text, true);
            }
            catch
            {
                return false;
            }
        }

        // 修正：去掉 C# 8.0 的可空引用类型注解 '?'
        private static string ResolveLibraryPath(string fileName)
        {
            var direct = Path.Combine(AppPaths.BaseDirectory, fileName);
            if (File.Exists(direct))
            {
                return direct;
            }

            var readers = Path.Combine(AppPaths.ReadersDirectory, fileName);
            if (File.Exists(readers))
            {
                return readers;
            }

            return null;
        }

        private static bool EnsureLoaded(string libraryPath)
        {
            try
            {
                return Kernel32.LoadLibrary(libraryPath) != IntPtr.Zero;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (_tolkLoaded)
            {
                try
                {
                    TolkNative.Tolk_Unload();
                }
                catch
                {
                    // Ignore unload failures.
                }
            }

            _fallbackSynth.Dispose();
        }

        private static class TolkNative
        {
            [DllImport("Tolk.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool Tolk_Load();

            [DllImport("Tolk.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
            public static extern void Tolk_Unload();

            [DllImport("Tolk.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool Tolk_Output([MarshalAs(UnmanagedType.LPWStr)] string text, [MarshalAs(UnmanagedType.Bool)] bool interrupt);
        }

        private static class Kernel32
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);
        }
    }
}