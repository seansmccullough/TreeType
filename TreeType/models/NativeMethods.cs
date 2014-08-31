namespace VirtualInput
{
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class MouseHookStruct
        {
            public Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //system calls to set hook, unhook, and call next hook in chain if passing message on

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        internal delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        //synthesizes keyboard input
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        //used to get screen resolution
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        //synthesizes mouse input
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);

        //used to remove red x from windows
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        //moves mouse to designated position (in pixels)
        public static bool moveMouse(int x, int y)
        {
            mouse_event(WM_MOUSE_ABSOLUTE | WM_MOUSEMOVE, 
                (uint)(x * (65536 / GetSystemMetrics(X_SCREEN))), 
                (uint)(y * (65536 / GetSystemMetrics(Y_SCREEN))), 
                0, 
                UIntPtr.Zero);
            return true;
        }

        public static void type(string s)
        {
            for(int i=0; i<s.Length; i++)
            {
                KeyPress(s[i]);
            }
        }

        public static void KeyDown(char key)
        {
            keybd_event((byte)key, 0x45, 1 | 0, 0);
        }

        public static void KeyUp(char key)
        {
            keybd_event((byte)key, 0x45, 1 | 0x0002, 0);
        }

        public static void KeyPress(char character)
        {
            byte key = Convert.ToByte(character-0x20);
            keybd_event(key, 0x45, 1 | 0, 0);
            keybd_event(key, 0x45, 1 | 0x0002, 0);
        }

        public static void KeyPress(Byte key)
        {
            keybd_event(key, 0x45, 1 | 0, 0);
            keybd_event(key, 0x45, 1 | 0x0002, 0);
        }

        public static void KeyPress(Keys key)
        {
            KeyPress((char)key);
        }

        internal const int GWL_STYLE = -16;
        internal const int WS_SYSMENU = 0x80000;

        internal const int WH_MOUSE_LL = 14;
        internal const int WH_KEYBOARD_LL = 13;

        internal const int X_SCREEN = 0;
        internal const int Y_SCREEN = 1;

        internal const int WH_MOUSE = 7;
        internal const int WH_KEYBOARD = 2;

        internal const int WM_MOUSE_ABSOLUTE = 0x8000;
        internal const int WM_MOUSEMOVE = 0x0001;
        internal const int WM_LBUTTONDOWN = 0x201;
        internal const int WM_RBUTTONDOWN = 0x204;
        internal const int WM_MBUTTONDOWN = 0x207;
        internal const int WM_LBUTTONUP = 0x202;
        internal const int WM_RBUTTONUP = 0x205;
        internal const int WM_MBUTTONUP = 0x208;
        internal const int WM_LBUTTONDBLCLK = 0x203;
        internal const int WM_RBUTTONDBLCLK = 0x206;
        internal const int WM_MBUTTONDBLCLK = 0x209;
        internal const int WM_MOUSEWHEEL = 0x020A;

        internal const int WM_KEYDOWN = 0x100;
        internal const int WM_KEYUP = 0x101;
        internal const int WM_SYSKEYDOWN = 0x104;
        internal const int WM_SYSKEYUP = 0x105;

        internal const byte VK_SHIFT = 0x10;
        internal const byte VK_CAPITAL = 0x14;
        internal const byte VK_NUMLOCK = 0x90;
    }
}
