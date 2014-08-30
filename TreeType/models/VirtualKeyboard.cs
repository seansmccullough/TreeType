namespace VirtualInput
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public delegate bool Win32KeyPress(Keys key);
    public delegate bool MouseEvent(MSLLHOOKSTRUCT e, Int32 wParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>Enables applications to intercept keystrokes</summary>
    public class VirtualKeyboard
    {
        private static int hHook = 0;
        private static NativeMethods.HookProc hookProc;

        public static event Win32KeyPress keyPressHandler;
        public static event MouseEvent mouseEventHandler;
        public static int TreeTypeChar;



        /// <summary>Starts intercepting keystrokes</summary>
        public static void StartKeyboardInterceptor()
        {
            if (hHook == 0)
            {
                hookProc = new NativeMethods.HookProc(KeyboardHookProc);
                hHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, hookProc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                TreeTypeChar = 0;
                if (hHook == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    StopInterceptor();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>Stops intercepting user input</summary>
        public static void StopInterceptor()
        {
            if (hHook != 0)
            {
                int result = NativeMethods.UnhookWindowsHookEx(hHook);
                hHook = 0;
                if (result == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>Called by the Win32 functions when a keyboard event occurs</summary>
        /// <param name="nCode">A code the hook procedure uses to determine how to process the message. If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</param>
        /// <param name="wParam">The virtual-key code of the key that generated the keystroke message.</param>
        /// <param name="lParam">The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.</param>
        /// <returns>1 if the event was handled, otherwise the return value of CallNextHookEx.</returns>
        private static int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            bool handled = false;
            TreeTypeChar = 0;

            if ((nCode >= 0) && (keyPressHandler != null))
            {
                NativeMethods.KeyboardHookStruct MyKeyboardHookStruct = (NativeMethods.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(NativeMethods.KeyboardHookStruct));

                if ((keyPressHandler != null) && (wParam == NativeMethods.WM_KEYDOWN || wParam == NativeMethods.WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    handled = keyPressHandler(keyData);
                }
            }

            return handled ? 1 : NativeMethods.CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>Starts intercepting mouse</summary>
        public static void StartMouseInterceptor()
        {
            if (hHook == 0)
            {
                hookProc = new NativeMethods.HookProc(MouseHookProc);
                hHook = NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, hookProc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                TreeTypeChar = 0;
                if (hHook == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    StopInterceptor();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        //capture mouse input
        //lParam contains the mouse message
        //see http://msdn.microsoft.com/en-us/library/windows/desktop/ms644970(v=vs.85).aspx
        private static int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            bool handled = false;
            MSLLHOOKSTRUCT myMouseEvent = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            handled = mouseEventHandler(myMouseEvent, wParam);
            //this is an if statement.  condition : true : false
            return handled ? 1 : NativeMethods.CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
}