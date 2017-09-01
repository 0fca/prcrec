using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ProcessRecorder.Pages
{
    public partial class RecordView : System.Windows.Controls.UserControl
    {
       
        BrushConverter bc = new BrushConverter();
        bool isOn = false;
        NativeControllers.NativeController n = new NativeControllers.NativeController();
        public RecordView()
        {
            InitializeComponent();
            n.PropertyChanged += new PropertyChangedEventHandler(SetDataToView);
        }

        internal void SetDataToView(object sender, PropertyChangedEventArgs e) {
           
           infoLabel.Text = n.MouseLocationData;
           titleLabel.Content = n.MouseData;
           keyboardInputTextBlock.Text = n.KeyboardData;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!isOn)
            {
                n.InitProcedures();
                Task task = new Task(() => IfAnyKeyPressed());
                //task.Start();
                WinApi.User.Window.GetWindowThreadProcessId(NativeControllers.NativeController.GetActvHwnd(), out uint threadId);
                n.SetActiveHandles(threadId);
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
            }
            isOn = !isOn;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isOn)
            {
                n.Stop();
               
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

        public void IfAnyKeyPressed() {
            while (true) {
                byte[] b = new byte[256];
                bool isAnyError = WinApi.User.Input.GetKeyboardState(b);

                if (isAnyError)
                {
                    //for (int i = 0; i < 256; i++) {
                    //    byte key = b[i];
                    //    //Debug.Write(key);
                    //    //if (key == 0)
                    //    //{
                    //    //    MOUSE_POS_TRACK.Add("[_break_]");
                    //    //    Debug.WriteLine("break");
                    //    //}
                    //    if (i == 255) {
                    //        //Debug.WriteLine("");
                    //    }
                    //}
                }
                else
                {
                    Debug.WriteLine("Error..."+WinApi.User.Input.GetLastError());
                }
            }
        }
    }
}
