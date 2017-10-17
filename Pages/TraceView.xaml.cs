using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProcessRecorder.Images;

namespace ProcessRecorder.Pages
{
    /// <summary>
    /// Interaction logic for TraceView.xaml
    /// </summary>
    public partial class TraceView : UserControl
    {
        public TraceView()
        {
            InitializeComponent();
            traceListView.ItemsSource = ImageContainer.GetImages();
            
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //Debug.WriteLine("Scroll.");
        }

        private void MouseWheelListener(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Debug.WriteLine("Scrolled.");
        }
    }

    internal class ImageContainer {
        internal static ObservableCollection<ImageData> GetImages()
        {
            ObservableCollection<ImageData> results = new ObservableCollection<ImageData>();
            if (Directory.Exists(ImageCapturer.TMP_DIR))
            {
                string[] files = Directory.GetFiles(ImageCapturer.TMP_DIR);
                foreach (string card in files)
                {
                    string name = Path.GetFileName(card);
                    ImageData im = new ImageData();
                    im.ImageName = name;
                    im.ImageFullPath = card;
                    results.Add(im);
                }
            }
            else
            {
                Directory.CreateDirectory(Images.ImageCapturer.TMP_DIR);
            }
            return results;
        }
    }
}
