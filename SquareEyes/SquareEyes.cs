using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SquareEyes
{
    public partial class SquareEyes : Form
    {
        public SquareEyes()
        {
            InitializeComponent();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void SquareEyes_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void SquareEyes_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(UpdateTime);
            thread.Start();
        }
        void UpdateTime()
        {
            TimeSpan t = new TimeSpan();
            Stopwatch s = new Stopwatch();

            while (true)
            {
                if (IdleTimeFinder.GetIdleTime() > 60000)
                {
                    s.Stop();
                }
                else
                {
                    s.Start();
                    t = TimeSpan.FromMilliseconds(s.ElapsedMilliseconds);
                    string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                            t.Hours,
                                            t.Minutes,
                                            t.Seconds);
                    lblUsage.Invoke((MethodInvoker)(() => lblUsage.Text = answer));
                }

                Thread.Sleep(1000);
            }
        }

        private void SquareEyes_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            tmrExpand.Enabled = true;
        }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }

    /// <summary>
    /// Helps to find the idle time, (in milliseconds) spent since the last user input
    /// </summary>
    public class IdleTimeFinder
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }
    }
}
