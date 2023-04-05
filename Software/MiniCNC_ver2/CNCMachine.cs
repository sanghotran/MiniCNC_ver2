using System;
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
        public string[] DataReceive;
        public int State;
    }
}
