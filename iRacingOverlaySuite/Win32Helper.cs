using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iRacingOverlaySuite
{
    // Define a RECT struct to be used with GetWindowRect from user32.dll
    public struct RECT { public int left, top, right, bottom, width, height; }

    public struct POINT
    {
        public Int32 X;
        public Int32 Y;
    }

    public static class Win32Helper
    {
        [DllImport("user32.dll", SetLastError = true)] // Unmanaged code
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}
