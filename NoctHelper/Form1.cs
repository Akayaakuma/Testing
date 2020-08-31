using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoctHelper
{
    public partial class Form1 : Form
    {

        [DllImport(dllName:"user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hwnd);


        [DllImport("user32.dll")]
        private static extern int GetWindowTextW([In] IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] [Out]
            StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId([In] IntPtr hWnd, ref int lpdwProcessId);

        private const int LEFTUP = 0x0004;
        private const int LEFTDOWN = 0x0002;

        private string currentWindow = "";
        private int currentPID;
        private int mcpid;

        public Form1()
        {
            InitializeComponent();
        }

        public void GetForeGroundWindowInfo()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
                if (!foregroundWindow.Equals(IntPtr.Zero))
            {
                int windowTextLength = GetWindowTextLength(foregroundWindow);
                StringBuilder stringBuilder = new StringBuilder("", windowTextLength +1);
                if (windowTextLength > 0)
                {
                    GetWindowTextW(foregroundWindow, stringBuilder, stringBuilder.Capacity);
                }

                int currentpid = 0;
                GetWindowThreadProcessId(foregroundWindow, ref currentpid);
                Process[] processByName = Process.GetProcessesByName("javaw");
                foreach (Process process in processByName)
                {
                    mcpid = process.Id;
                }

                currentWindow = stringBuilder.ToString();
                currentPID = currentpid;
            }
        }


        private void clicktimer_Tick(object sender, EventArgs e)
        {
            GetForeGroundWindowInfo();
            Random rnd = new Random();
            int maxcps = (int)Math.Round(1000.0 / (trackBar1.Value + trackBar2.Value * 0.2));
            int mincps = (int)Math.Round(1000.0 / (trackBar1.Value + trackBar2.Value * 0.4));
            try
            {
                clicktimer.Interval = rnd.Next(mincps, maxcps);
            }
            catch
            {
                //Ignored
            }

            if(currentPID == Process.GetCurrentProcess().Id)
            {
                return;
            }

            if (checkBox1.Checked && currentPID != mcpid && mcpid !=0)
            {
                return;
            }

            bool mousdown = MouseButtons == MouseButtons.Left;
            if (mousdown)
            {
                mouse_event(dwFlags: LEFTUP, dx: 0, dy: 0, cButtons: 0, dwExtraInfo: 0);
                Thread.Sleep(millisecondsTimeout: rnd.Next(1, 6));
                mouse_event(dwFlags: LEFTDOWN, dx: 0, dy: 0, cButtons: 0, dwExtraInfo: 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Contains("on"))
            {
                clicktimer.Start();
                button1.Text = "Toggle: off";
            }
            else
            {
                clicktimer.Stop();
                button1.Text = "Toggle: on";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = $"Max Cps: {trackBar1.Value}";
        }

        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            label2.Text = $"Min Cps: {trackBar2.Value}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string exename = AppDomain.CurrentDomain.FriendlyName;
            DirectoryInfo d = new DirectoryInfo(path: @"C:\Windows\Prefetch");
            FileInfo[] Files = d.GetFiles(searchPattern: exename + "*");
            foreach (FileInfo file in Files)
            {
                File.Delete(file.FullName);
            }

            Environment.Exit(0);

        }
    }
}
