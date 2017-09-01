using System.Windows.Controls;
using System.Windows.Input;

namespace ProcessRecorder.Pages
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        private void LinkTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.github.com/Obsidiam/prcrec");
        }
    }
}
