using ProcessRecorder.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using static ProcessRecorder.Model.TracesModel;

namespace ProcessRecorder.NativeControllers
{
    internal class NativeController 
    {
        private IntPtr MW;
        private static IntPtr ACTV_PRC_HWND = Win32.User.Hook.LoadLibrary("user32.dll");
        private string ACT_WND_TITLE = "";
        Win32.User.Hook.HookProc MouseHookProcedure;
        Win32.User.Hook.HookProc KeyboardHookProcedure;
        int hHook, kHook;
        private Images.ImageCapturer i = new Images.ImageCapturer();
        private MouseStruct mouseStruct = new MouseStruct();
        private KeyboardStruct keyboardStruct = new KeyboardStruct();
        private InputDataModel idm;


        public NativeController(InputDataModel inputDataModel)
        {
            this.idm = inputDataModel;
            MW = Win32.User.Window.FindWindow(null, "MacroPath");
        }

        private string mouse;


        public void InitProcedures()
        {
            MouseHookProcedure = new Win32.User.Hook.HookProc(MouseHookProc);
            KeyboardHookProcedure = new Win32.User.Hook.HookProc(KeyboardHookProc);
        }

        public void InitStructures() {
           
        }

        public void SetActiveHandles(uint threadId)
        {
            hHook = Win32.User.Hook.SetWindowsHookEx(Win32.User.Hook.WH_MOUSE_LL, MouseHookProcedure, ACTV_PRC_HWND, threadId);
            kHook = Win32.User.Hook.SetWindowsHookEx(Win32.User.Hook.WH_KEYBOARD_LL, KeyboardHookProcedure, ACTV_PRC_HWND, threadId);
        }

        public int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Win32.User.Hook.MouseHookStruct MouseHookStruct = (Win32.User.Hook.MouseHookStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(Win32.User.Hook.MouseHookStruct));
            if (nCode < 0)
            {
                return Win32.User.Hook.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                IntPtr FRG_HWND = Win32.User.Window.GetForegroundWindow();
                ACTV_PRC_HWND = FRG_HWND;
                string onScr, onWnd = "" ;
                onScr = "On Screen: " + wParam + " : " + MouseHookStruct.pt.x + "," + MouseHookStruct.pt.y;

                Win32.User.Window.GetCursorPos(out MouseHookStruct.pt);
                Win32.User.Window.ScreenToClient(FRG_HWND, ref MouseHookStruct.pt);
                onWnd += "On Window: " + Win32.User.InputFormat.InputDataFormater.GetMsgStringRep((uint)(wParam), typeof(Win32.User.Enumeration.MouseButtons)) + " : " + MouseHookStruct.pt.x + "," + MouseHookStruct.pt.y;
                string loc = string.Format("{0}\n{1}", onScr, onWnd);
                StringBuilder sb = new StringBuilder(Win32.User.Window.GetWindowTextLength(FRG_HWND) + 1);
                Win32.User.Window.GetWindowText(FRG_HWND, sb, sb.Capacity);
                ACT_WND_TITLE = sb.ToString();
                idm.KeyboardData = "No text is entered.";
                mouse = "Focused window is " + ACT_WND_TITLE;
                idm.MouseData = mouse;
                idm.MouseLocationData = loc;

                if ((int)wParam == Win32.User.Hook.WM_LBUTTONDOWN | (int)wParam == Win32.User.Hook.WM_RBUTTONDOWN)
                {
                    mouseStruct.Add(wParam.ToInt32().ToString() + ":" + MouseHookStruct.pt.x + ":" + MouseHookStruct.pt.y);
                }

                if (mouseStruct.getSize() == 100)
                {
                    ExecuteWritingThread();
                }

                if ((int)wParam == 513 || (int)wParam == 516)
                {
                    Images.ImageCapturer.CaptureTheScreenImage(MouseHookStruct.pt.x, MouseHookStruct.pt.y);
                }
                return Win32.User.Hook.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Win32.User.Hook.keyboardHookStruct KeyStruct = (Win32.User.Hook.keyboardHookStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(Win32.User.Hook.keyboardHookStruct));

            if (nCode < 0)
            {
                return Win32.User.Hook.CallNextHookEx(kHook, nCode, wParam, lParam);
            }
            else
            {
                if (ACTV_PRC_HWND != MW)
                {
                    int vkCode = KeyStruct.vkCode;
                    char ch = Win32.User.InputFormat.InputDataFormater.TranslateVirtualKeyIntoChar((uint)vkCode);
                    byte[] getState = new byte[256];
                    bool isAnyDown = Win32.User.Input.GetKeyboardState(getState);

                    for (int i = 0; i < 16; i++)
                    {
                        byte key = getState[i];

                        if ((key & 1) == 1)
                        {

                            string value = Win32.User.InputFormat.InputDataFormater.TranslateVirtualKeyToString((uint)vkCode, ch);
                            if (value.Length > 1)
                            {
                                value = "[" + value + "]";
                            }

                            keyboardStruct.Add(value);
                            idm.KeyboardData = "Text is entered to " + ACT_WND_TITLE;

                        }
                    }
                    //Debug.WriteLine(keyboard);
                }
                return Win32.User.Hook.CallNextHookEx(kHook, nCode, wParam, lParam);
            }
        }

        public static IntPtr GetActvHwnd() {
            return ACTV_PRC_HWND;
        }


        internal void Stop()
        {
            Win32.User.Hook.UnhookWindowsHookEx(hHook);
            Win32.User.Hook.UnhookWindowsHookEx(kHook);
            ACTV_PRC_HWND = IntPtr.Zero;
        }

        private void ExecuteWritingThread()
        {
            Thread task = new Thread(WriteData);
            task.Start();
            //Thread image_capturer = new Thread();
        }
    }
}
