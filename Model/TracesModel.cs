using System;
using System.Collections.Generic;


namespace ProcessRecorder.Model
{
    internal class TracesModel
    {
        static List<string> MOUSE_POS_TRACK = new List<string>();
        static List<string> KEYBOARD_STATE_TRACK = new List<string>();

        private static Type type;
        internal static void SetType(Type typeIn) {
            type = typeIn;
        }

        internal struct MouseStruct {
            private int pointer;

            internal int getSize()
            {
                return MOUSE_POS_TRACK.Count;
            }

            internal bool shouldSave()
            {
                if (pointer > 100)
                {
                    pointer = 0;
                    return true;
                }
                return false;
            }

            internal void Add(string s) {
                MOUSE_POS_TRACK.Add(s);
            }
        };

        internal struct KeyboardStruct {
            private int pointer;

            internal int getSize() {
                return KEYBOARD_STATE_TRACK.Count;
            }

            internal bool ShouldSave() {
                if (pointer > 100) {
                    pointer = 0;
                    return true;
                }
                return false;
            }

            internal void Add(string s)
            {
                KEYBOARD_STATE_TRACK.Add(s);
            }
        };

        internal static void WriteData() {
            if (type == typeof(MouseStruct)) {

            }

            if (type == typeof(KeyboardStruct)) {

            }
        }
    }
}
