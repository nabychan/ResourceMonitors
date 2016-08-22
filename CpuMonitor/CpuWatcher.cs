using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Runtime.InteropServices;
using System.Management;

namespace CpuMonitor
{   
    public class CpuWatcher : IDisposable
    {
        private PerformanceCounter _PerformanceCounter;
        private bool _Watch;
        private Task _Task;
        #region AIP声明 
        [DllImport("IpHlpApi.dll")]
        extern static public uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        [DllImport("User32")]
        private extern static int GetWindow(int hWnd, int wCmd);

        [DllImport("User32")]
        private extern static int GetWindowLongA(int hWnd, int wIndx);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int GetWindowTextLength(IntPtr hWnd);
        #endregion
        public CpuWatcher()
        {
            _PerformanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _PerformanceCounter.MachineName = ".";
            _PerformanceCounter.NextValue();
            _Watch = false;
        }

        public void Watch(int seconds)
        {
            _Watch = true;
            _Task = new TaskFactory().StartNew(() =>
              {
                  while (_Watch)
                  {
                      (App.Current as App).Refresh((int)_PerformanceCounter.NextValue());
                      //(App.Current as App).NotifyIcon.BeginInvoke(
                      //    DispatcherPriority.Background,
                      //    new Action<int>(p =>
                      //    {
                      //        App.Current.Shutdown();
                      //        MessageBox.Show("sfsd");
                      //        //(App.Current as App).Refresh(p);
                      //    }),
                      //    _PerformanceCounter.NextValue());
                      System.Threading.Thread.Sleep(seconds);
                  }
              });
        }
        public void StopWatch()
        {
            _Watch = false;
            if (_Task != null)
            {
                _Task.Wait();
                _Task = null;
            }
        }

        public void Dispose()
        {

            StopWatch();
            _PerformanceCounter.Dispose();
        }
    }
}
