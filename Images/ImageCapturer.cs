using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System;

namespace ProcessRecorder.Images
{
   internal sealed class ImageCapturer
    {
        public static string USER_NAME  = System.Security.Principal.WindowsIdentity.GetCurrent().Name;   
        public static string TMP_DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\Local\\PrcRecTmp";

        public static void CaptureTheScreenImage(int x, int y) {
            if (CheckIfTmpExists())
            {
                DoScreenCapture(x,y);
            }
            else
            {
                Directory.CreateDirectory(TMP_DIR);
                DoScreenCapture(x,y);
            }
        }

        private static void DoScreenCapture(int x, int y)
        {
            Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bmpScreenCapture);
                
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                     Screen.PrimaryScreen.Bounds.Y,
                                     0, 0,
                                     bmpScreenCapture.Size,
                                     CopyPixelOperation.SourceCopy);
                
                Rectangle section = new Rectangle(new Point(x, y), new Size(250, 150));
                CropImage(bmpScreenCapture, section).Save(TMP_DIR+"\\img"+x+"_"+y+".jpg");
                GC.Collect();
        }

        private static bool CheckIfTmpExists()
        {
            if (!Directory.Exists(TMP_DIR))
            {
                return false;
            }
            return true;
        }

        private static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
        }

        public string getImageDir() {
            return TMP_DIR;
        }
    }
}
