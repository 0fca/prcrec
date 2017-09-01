using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessRecorder.NativeControllers
{
    internal class NativeController : INotifyPropertyChanged
    {
        private IntPtr MW;
        private static IntPtr ACTV_PRC_HWND = WinApi.Win32.User.Hook.LoadLibrary("user32.dll");
        private string ACT_WND_TITLE = "";
        WinApi.Win32.User.Hook.HookProc MouseHookProcedure;
        WinApi.Win32.User.Hook.HookProc KeyboardHookProcedure;
        int hHook, kHook;
        private Images.ImageCapturer i = new Images.ImageCapturer();

        List<string> MOUSE_POS_TRACK = new List<string>(), KEYBOARD_STATE_TRACK = new List<string>();
        private static string keyboard,mouse,loc;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }


        public NativeController(){
            MW = WinApi.User.Window.FindWindow(null, "MacroPath");
        }

        public void InitProcedures()
        {
            MouseHookProcedure = new WinApi.Win32.User.Hook.HookProc(MouseHookProc);
            KeyboardHookProcedure = new WinApi.Win32.User.Hook.HookProc(KeyboardHookProc);
        }

        public void SetActiveHandles(uint threadId)
        {
            hHook = WinApi.Win32.User.Hook.SetWindowsHookEx(WinApi.Win32.User.Hook.WH_MOUSE_LL, MouseHookProcedure, ACTV_PRC_HWND, threadId);
            kHook = WinApi.Win32.User.Hook.SetWindowsHookEx(WinApi.Win32.User.Hook.WH_KEYBOARD_LL, KeyboardHookProcedure, ACTV_PRC_HWND, threadId);
        }

        public int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            WinApi.Win32.User.Hook.MouseHookStruct MyMouseHookStruct = (WinApi.Win32.User.Hook.MouseHookStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(WinApi.Win32.User.Hook.MouseHookStruct));
            if (nCode < 0)
            {
                return WinApi.Win32.User.Hook.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                IntPtr FRG_HWND = WinApi.User.Window.GetForegroundWindow();
                ACTV_PRC_HWND = FRG_HWND;
                string onScr, onWnd = "" ;
                onScr = "On Screen: " + wParam + " : " + MyMouseHookStruct.pt.x + "," + MyMouseHookStruct.pt.y;

                WinApi.User.Window.GetCursorPos(out MyMouseHookStruct.pt);
                WinApi.User.Window.ScreenToClient(FRG_HWND, ref MyMouseHookStruct.pt);
                onWnd += "On Window: " + GetMsgStringRep((uint)(wParam), typeof(WinApi.User.Enumeration.MouseButtons)) + " : " + MyMouseHookStruct.pt.x + "," + MyMouseHookStruct.pt.y;
                loc = string.Format("{0}\n{1}", onScr, onWnd);
                StringBuilder sb = new StringBuilder(WinApi.User.Window.GetWindowTextLength(FRG_HWND) + 1);
                WinApi.User.Window.GetWindowText(FRG_HWND, sb, sb.Capacity);
                ACT_WND_TITLE = sb.ToString();
                KeyboardData = "No text is entered.";
                mouse = "Focused window is " + ACT_WND_TITLE;
                MouseData = mouse;
                MouseLocationData = loc;

                if ((int)wParam == WinApi.Win32.User.Hook.WM_LBUTTONDOWN | (int)wParam == WinApi.Win32.User.Hook.WM_RBUTTONDOWN)
                {
                    MOUSE_POS_TRACK.Add(wParam.ToInt32().ToString() + ":" + MyMouseHookStruct.pt.x + ":" + MyMouseHookStruct.pt.y);
                }

                if (MOUSE_POS_TRACK.Count == 100)
                {
                    ExecuteWritingThread();
                }
                return WinApi.Win32.User.Hook.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            WinApi.Win32.User.Hook.keyboardHookStruct KeyStruct = (WinApi.Win32.User.Hook.keyboardHookStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(WinApi.Win32.User.Hook.keyboardHookStruct));

            if (nCode < 0)
            {
                return WinApi.Win32.User.Hook.CallNextHookEx(kHook, nCode, wParam, lParam);
            }
            else
            {
                if (ACTV_PRC_HWND != MW)
                {
                    int vkCode = KeyStruct.vkCode;
                    char ch = TranslateVirtualKeyIntoChar((uint)vkCode);
                    byte[] getState = new byte[256];
                    bool isAnyDown = WinApi.User.Input.GetKeyboardState(getState);

                    for (int i = 0; i < 16; i++)
                    {
                        byte key = getState[i];

                        if ((key & 1) == 1)
                        {

                            string value = TranslateVirtualKeyToString((uint)vkCode, ch);
                            if (value.Length > 1)
                            {
                                value = "[" + value + "]";
                            }

                            KEYBOARD_STATE_TRACK.Add(value);
                            KeyboardData = "Text is entered to " + ACT_WND_TITLE;

                        }
                    }
                    //Debug.WriteLine(keyboard);
                }
                return WinApi.Win32.User.Hook.CallNextHookEx(kHook, nCode, wParam, lParam);
            }
        }

        public static IntPtr GetActvHwnd() {
            return ACTV_PRC_HWND;
        }

        private string TranslateVirtualKeyToString(uint key, char c)
        {

            if (key >= 65 & key <= 122)
            {
                if (IsShiftKeyDown())
                {
                    return c.ToString().ToUpper();
                }
                else
                {
                    return c.ToString().ToLower();
                }
            }
            else
            {
                return GetMsgStringRep(key, typeof(WinApi.User.Enumeration.VirtualKeys));
            }
        }

        private char TranslateVirtualKeyIntoChar(uint key)
        {
            char c = (char)WinApi.User.Input.MapVirtualKey(key, WinApi.User.Input.MAPVK_VK_TO_CHAR);

            return c;
        }

        private string GetMsgStringRep(uint key, Type t)
        {
            
            System.Reflection.FieldInfo[] fields = t.GetFields();

            foreach (var field in fields)
            {
                if (field.Name.Equals("value__")) continue;

                if (UInt32.Parse((field.GetRawConstantValue().ToString())) == key)
                {
                    return field.Name;
                }
            }
            return "";
        }

        

        private bool IsShiftKeyDown()
        {
            short shiftState = WinApi.User.Input.GetAsyncKeyState((int)WinApi.User.Input.VK_SHIFT);
            if (shiftState != 0)
            {
                return true;
            }
            return false;
        }

        private void WriteMouseData()
        {
            String filePath = "C:\\Users\\lukas\\Desktop\\mouse_macro.csv";
            StringBuilder csv = new StringBuilder();

            foreach (string line in MOUSE_POS_TRACK)
            {
                string newLine = string.Format("{0},{1}", Images.ImageCapturer.TMP_DIR + "\\" + line, line);
                csv.AppendLine(newLine);
            }
            Debug.WriteLine("CSV: " + csv);
            File.WriteAllText(filePath, csv.ToString());
            MOUSE_POS_TRACK.Clear();

        }

        private void CaptureImage(int x, int y)
        {
            i.CaptureTheScreenImage(x, y);
        }


        internal void Stop()
        {
            WinApi.Win32.User.Hook.UnhookWindowsHookEx(hHook);
            WinApi.Win32.User.Hook.UnhookWindowsHookEx(kHook);
            ACTV_PRC_HWND = IntPtr.Zero;
        }

        private void ExecuteWritingThread()
        {
            Thread task = new Thread(WriteMouseData);
            task.Start();
            //Thread image_capturer = new Thread();
        }

        public string MouseData {
            get {
                return mouse;
            }
            set
            {
                if (value != mouse)
                {
                    mouse = value;
                    NotifyPropertyChanged(MouseData.GetType().Name);
                }
            }
        }

        public string KeyboardData {
            get
            {
                return keyboard;
            }
            set
            { 
                    keyboard = value;
                    NotifyPropertyChanged(KeyboardData.GetType().Name);
            }
        }

        public string MouseLocationData {
            get
            {
                return loc;
            }
            set
            {
                if (value != loc)
                {
                    loc = value;
                    NotifyPropertyChanged(MouseLocationData.GetType().Name);
                }
            }
        }
    }
}
