using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace MiniCNC_ver2
{
    public class CNCMachine
    {

        public const int CNC_PID = 22370;
        public const int CNC_VID = 1115;
        private bool _autoCheckConnet { get; set; }

        public void AutoCheckConnect(bool state)
        {   
            _autoCheckConnet = state;
            if(_autoCheckConnet)
            {
                Thread autoCheckConnect = new Thread(checkConnect);
                autoCheckConnect.Start();
            }    
        }

        private void checkConnect()
        {
            while (_autoCheckConnet)
            {
                if (!MainWindow.myUsbDevice.IsOpen)
                {

                }
                Thread.Sleep(1000);
            }
        }
    }
}
