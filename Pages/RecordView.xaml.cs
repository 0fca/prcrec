using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace ProcessRecorder.Pages
{
    public partial class RecordView : System.Windows.Controls.UserControl
    {
        private IntPtr MW;
        private IntPtr ACTV_PRC_HWND = WinApi.Win32.User.Hook.LoadLibrary("user32.dll");
        private string ACT_WND_TITLE = "";
        WinApi.Win32.User.Hook.HookProc MouseHookProcedure;
        WinApi.Win32.User.Hook.HookProc KeyboardHookProcedure;
        int hHook, kHook;
        private Images.Image i = new Images.Image();
        List<string> MOUSE_POS_TRACK = new List<string>(), KEYBOARD_STATE_TRACK = new List<string>();
        BrushConverter bc = new BrushConverter();
        bool isOn = false;

        public RecordView()
        {
            InitializeComponent();
            MW = WinApi.User.Window.FindWindow(null, "MacroPath");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!isOn)
            {
                InitProcedures();
                WinApi.User.Window.GetWindowThreadProcessId(ACTV_PRC_HWND, out uint threadId);
                SetActiveHandles(threadId);
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
            }
            isOn = !isOn;
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
                string onScr, onWnd = "";
                onScr = "On Screen: " + wParam + " : " + MyMouseHookStruct.pt.x + "," + MyMouseHookStruct.pt.y;
                
                WinApi.User.Window.GetCursorPos(out MyMouseHookStruct.pt);
                WinApi.User.Window.ScreenToClient(FRG_HWND, ref MyMouseHookStruct.pt);
                onWnd += "On Window: " + wParam + " : " + MyMouseHookStruct.pt.x + "," + MyMouseHookStruct.pt.y;
                infoLabel.Text = string.Format("{0}\n{1}", onScr, onWnd);
                StringBuilder sb = new StringBuilder(WinApi.User.Window.GetWindowTextLength(FRG_HWND) + 1);
                WinApi.User.Window.GetWindowText(FRG_HWND, sb, sb.Capacity);
                ACT_WND_TITLE = sb.ToString();

                titleLabel.Content = "Focused window is "+ACT_WND_TITLE;

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
                    WinApi.User.Input.GetKeyboardState(getState);
                    //Debug.WriteLine(getState);
                    
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
                            Debug.Write(value);
                            keyboardInputTextBlock.Text = "Text is entered to window " + ACT_WND_TITLE + ".";
                        }
                        else if((key & 1) == 0)
                        {
                                keyboardInputTextBlock.Text = "No text is entered now.";
                        }
                    }

                    //if ((int)wParam == 260 && canAdd)
                    //{
                    //    short altgr = WinApi.User.Input.GetAsyncKeyState((int)WinApi.User.Input.VK_RMENU);
                    //    short alt = WinApi.User.Input.GetAsyncKeyState((int)WinApi.User.Input.VK_LMENU);

                    //    if ((altgr & 1) == 1 & (alt & 1) != 1)
                    //    {
                    //        Debug.Write("[Alt]");
                    //        KEYBOARD_STATE_TRACK.Add("[Alt]");
                    //    }

                    //    if ((alt & 1) == 1 & (altgr & 1) != 1)
                    //    {
                    //        Debug.Write("[AltGr]");
                    //        KEYBOARD_STATE_TRACK.Add("[AltGr]");
                    //    }
                    //    canAdd = false;
                    //}

                    //if ((int)wParam == 256 && canAdd)
                    //{
                    //    string text_rep = TranslateVirtualKeyToString((uint)vkCode, ch);
                    //    if (text_rep.Length == 1)
                    //    {
                    //        Debug.Write(text_rep);
                    //        KEYBOARD_STATE_TRACK.Add(text_rep);
                    //    }
                    //    else
                    //    {
                    //        Debug.Write("[" + text_rep + "]");
                    //        KEYBOARD_STATE_TRACK.Add("[" + text_rep + "]");
                    //    }
                    //    canAdd = false;
                    //}

                    //if ((int)wParam == 257 || (int)wParam == 270)
                    //{
                    //    canAdd = true;
                    //}
                }
                return WinApi.Win32.User.Hook.CallNextHookEx(kHook, nCode, wParam, lParam);
                }
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
                return GetModifierStringRep(key);
            }
        }

        private char TranslateVirtualKeyIntoChar(uint key)
        {
            char c = (char)WinApi.User.Input.MapVirtualKey(key, WinApi.User.Input.MAPVK_VK_TO_CHAR);

            return c;
        }

        private string GetModifierStringRep(uint key)
        {
            Type t = typeof(WinApi.User.Enumeration.VirtualKeys);
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

        private bool IsControlKeyDown()
        {
            short ctrlState = WinApi.User.Input.GetAsyncKeyState((int)WinApi.User.Input.VK_CONTROL);
            if (ctrlState != 0)
            {
                return true;
            }
            return false;
        }

        private bool IsAltDown()
        {
            short altState = WinApi.User.Input.GetAsyncKeyState((int)WinApi.User.Input.VK_MENU);
            if (altState != 0)
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
                string newLine = string.Format("{0},{1}", Images.Image.TMP_DIR + "\\" + line, line);
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


        private void Stop()
        {
            WinApi.Win32.User.Hook.UnhookWindowsHookEx(hHook);
            WinApi.Win32.User.Hook.UnhookWindowsHookEx(kHook);
        }

        private void ExecuteWritingThread()
        {
            Thread task = new Thread(WriteMouseData);
            task.Start();
            //Thread image_capturer = new Thread();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isOn)
            {
                Stop();
                ACTV_PRC_HWND = (IntPtr)0;
                startButton.IsEnabled = true;
                stopButton.IsEnabled = false;
            }
            isOn = !isOn;
        }

        private void ButtonMouseEntered(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button b = (System.Windows.Controls.Button)e.Source;
            b.Background = (Brush)bc.ConvertFrom("#FF03A9F4");
        }

        private void ButtonMouseLeaved(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button b = (System.Windows.Controls.Button)e.Source;
            b.Background = Brushes.Transparent;
        }

      
    }
}
