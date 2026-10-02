using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logokuma24062025
{
    internal static class Program
    {
        // Program aynı anda tek kopya çalışır; isimli Mutex'i ilk açılan kopya tutar
        private const string TekKopyaAdi = "Local\\RestoPOS_SiparisTakip_TekKopya";

        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool IsIconic(IntPtr hWnd);
        private const int SW_RESTORE = 9;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var tekKopya = new Mutex(true, TekKopyaAdi, out bool ilkKopya))
            {
                if (!ilkKopya)
                {
                    MessageBox.Show("Program zaten çalışıyor.", "RestoPOS Sipariş Takip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AcikPencereyiOneGetir();
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        // Açık kopya simge durumunda ya da başka pencerelerin arkasında kalmış olabilir; müşteri uyarıdan sonra onu görsün
        private static void AcikPencereyiOneGetir()
        {
            var bu = Process.GetCurrentProcess();
            var acik = Process.GetProcessesByName(bu.ProcessName).FirstOrDefault(p => p.Id != bu.Id && p.MainWindowHandle != IntPtr.Zero);
            if (acik == null) return;

            // Sadece simge durumundaysa geri getir; tam ekran pencere SW_RESTORE ile küçülürdü
            if (IsIconic(acik.MainWindowHandle))
                ShowWindow(acik.MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(acik.MainWindowHandle);
        }
    }
}
