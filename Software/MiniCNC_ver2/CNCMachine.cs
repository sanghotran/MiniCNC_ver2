/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace MiniCNC_ver2
{
    public class CNCMachine 
    {      
        public const int CNC_PID = 22370;
        public const int CNC_VID = 1115;
        private bool _autoCheckConnet { get; set; }
        private bool _warning { get; set; }

        public void AutoCheckConnect(bool state)
        {   
            _autoCheckConnet = state;
            if(_autoCheckConnet)
            {
                Thread autoCheckConnect = new Thread(checkConnect);
                autoCheckConnect.SetApartmentState(ApartmentState.STA);
                autoCheckConnect.Start();
            }    
        }

        private void checkConnect()
        {
            
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(DeviceRemoved);
            watcher.Start();
            while (_autoCheckConnet)
            {
                Thread.Sleep(1000);
            }
            watcher.Stop();            
        }

        private void DeviceRemoved(object sender, EventArrivedEventArgs e)
        {
            string query = "SELECT * FROM Win32_USBControllerDevice";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject usbDevice in searcher.Get())
            {
                // Kiểm tra xem thiết bị có phải là CNC Mini không
                if (usbDevice["Dependent"].ToString().Contains("5762"))
                {
                    return;
                }
            }
            _warning = true;
        }
    }
}
*/