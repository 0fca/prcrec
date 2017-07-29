using System;
using System.Text;
using System.Runtime.InteropServices;
using WinApi.User.Enumeration;

namespace WinApi.User
{
    public class Input
    {
        public const uint MAPVK_VK_TO_CHAR = 0x02;
        public const uint VK_SHIFT = 0x10;
        public const uint VK_CONTROL = 0x11;
        public const uint VK_MENU = 0x12;
        public const uint VK_RMENU = 0xA5;
        public const uint VK_LMENU = 0xA4;

        [StructLayout(LayoutKind.Sequential)]
        internal class POINT
        {
            public int x;
            public int y;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDATA {

        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct STATEDATA
        {
            [FieldOffset(0)] public short Value;
            [FieldOffset(0)] public byte Low;
            [FieldOffset(1)] public byte High;
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(VirtualKeys vKey);


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern int ToAscii(uint virtualKeyCode, uint scanCode,
            ref byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer, uint flags);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(uint nVirtKey);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError(); 
    }
}