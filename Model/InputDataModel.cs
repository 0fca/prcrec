using System.ComponentModel;

namespace ProcessRecorder.Model
{
    internal class InputDataModel : INotifyPropertyChanged
    {

        private static string keyboard, mouse, loc;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }


        public string KeyboardData
        {
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

        public string MouseLocationData
        {
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

        public string MouseData
        {
            get
            {
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

    }
}
