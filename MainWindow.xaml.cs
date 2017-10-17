using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ProcessRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private NotifyIcon n;
        public MainWindow()
        {
            InitializeComponent();
            CreateNotifyicon();
        }

        private void CreateNotifyicon()
        {
            Container components = new Container();
            System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem();
            System.Windows.Forms.MenuItem menuItemSettings = new System.Windows.Forms.MenuItem();

            menuItem1.Text = "Exit";
            menuItem1.Click += new EventHandler(MenuItem1_Click);

            menuItemSettings.Index = 0;
            menuItemSettings.Text = "Settings";
            menuItemSettings.Click += new EventHandler(SettingsMenuItemClick);
            contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { menuItemSettings, menuItem1 });

            n = new NotifyIcon(components);
            n.BalloonTipText = "MacroPath is working now in background.";

            n.ContextMenu = contextMenu1;

            n.Text = "MacroPath v.1.1";
            n.Visible = true;
            n.Icon = new Icon(SystemIcons.Information, 40, 40);
            n.Click += new EventHandler(NotifyIcon1_Click);

        }

        private void NotifyIcon1_Click(object Sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs m = (System.Windows.Forms.MouseEventArgs)e;
            if (m.Button == MouseButtons.Left)
            {
                Win32.User.Window.SetForegroundWindow(new WindowInteropHelper(this).Handle);
                this.WindowState = WindowState.Normal;
            }
        }

        private void MenuItem1_Click(object Sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void SettingsMenuItemClick(object sender, EventArgs e)
        {
            Win32.User.Window.SetForegroundWindow(new WindowInteropHelper(this).Handle);
            this.WindowState = WindowState.Normal;
        }

        private void MainWndStateChanged(object sender, EventArgs e)
        {
            switch (WindowState) {
                case WindowState.Minimized:
                    this.ShowInTaskbar = false;
                    n.Visible = true;
                    n.ShowBalloonTip(500);
                    break;
                case WindowState.Normal:
                    this.ShowInTaskbar = true;
                    n.Visible = false;
                    break;
            }
            
        }
    }
}
