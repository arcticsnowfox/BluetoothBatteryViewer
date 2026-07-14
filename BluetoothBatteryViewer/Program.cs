using System;
using System.Threading;
using System.Windows.Forms;

namespace BluetoothBatteryViewer
{
    internal static class Program
    {
        private static Mutex _singleInstanceMutex;

        [STAThread]
        static void Main()
        {
            bool createdNew = false;

            try
            {
                _singleInstanceMutex = new Mutex(
                    true,
                    @"Local\BluetoothBatteryViewer_SingleInstance",
                    out createdNew);

                if (!createdNew)
                {
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrayApplicationContext());
            }
            finally
            {
                if (createdNew && _singleInstanceMutex != null)
                {
                    _singleInstanceMutex.ReleaseMutex();
                }

                if (_singleInstanceMutex != null)
                {
                    _singleInstanceMutex.Dispose();
                }
            }
        }
    }
}