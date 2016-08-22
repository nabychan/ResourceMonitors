using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using Forms = System.Windows.Forms;
using System.Management;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace CpuMonitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Forms.NotifyIcon _NotifyIcon;
        private string _IconPath = @"..\..\CpuMonitor.ico";
        private CpuWatcher _Watcher;
        private Bitmap _State;
        public Forms.NotifyIcon NotifyIcon { get { return _NotifyIcon; } }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var createNew = false;
            var mutex = new Mutex(false, "", out createNew);
            if (!createNew)
            {
                MessageBox.Show("已经在运行！");
                Shutdown();
            }
            else
            {
                _NotifyIcon = new Forms.NotifyIcon();
                _NotifyIcon.Icon = new Icon(_IconPath);
                _NotifyIcon.Text = "Cpu占用率(%)";

                _State = new Bitmap(@"..\..\Monitor.bmp"); ;

                var contextMenu = new Forms.ContextMenu();

                var exitMenuItem = new Forms.MenuItem();
                exitMenuItem.Text = "退出";
                exitMenuItem.Click += new EventHandler(delegate { Shutdown(); });

                contextMenu.MenuItems.Add(exitMenuItem);

                _NotifyIcon.ContextMenu = contextMenu;
                _NotifyIcon.Visible = true;

                _Watcher = new CpuWatcher();
                _Watcher.Watch(2000);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_Watcher != null)
            {
                _Watcher.Dispose();
            }
            if (_NotifyIcon != null)
            {
                _NotifyIcon.Visible = false;
                _NotifyIcon.Dispose();
                _NotifyIcon = null;
            }            
        }

        public void Refresh(int percentage)
        {
            using (var g = Graphics.FromImage(_State))
            {
                g.Clear(Color.Transparent);
                g.DrawString(percentage.ToString(), new Font("Arial", 200, GraphicsUnit.Pixel), Brushes.White, new PointF(0, 0));
            }
            var icon = Icon.FromHandle(_State.GetHicon());
            _NotifyIcon.Icon = icon;
            _NotifyIcon.Visible = false;
            _NotifyIcon.Visible = true;
        }
    }
}
