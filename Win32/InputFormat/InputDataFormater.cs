using System;

namespace Win32.User.InputFormat
{
    internal sealed class InputDataFormater
    {

        internal static  string TranslateVirtualKeyToString(uint key, char c)
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
                return GetMsgStringRep(key, typeof(Win32.User.Enumeration.VirtualKeys));
            }
        }

        internal static char TranslateVirtualKeyIntoChar(uint key)
        {
            char c = (char)Win32.User.Input.MapVirtualKey(key, Win32.User.Input.MAPVK_VK_TO_CHAR);

            return c;
        }

        internal static string GetMsgStringRep(uint key, Type t)
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

        private static bool IsShiftKeyDown()
        {
            short shiftState = Win32.User.Input.GetAsyncKeyState((int)Win32.User.Input.VK_SHIFT);
            if (shiftState != 0)
            {
                return true;
            }
            return false;
        }
    }
}
