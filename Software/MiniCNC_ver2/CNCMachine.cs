using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniCNC_ver2
{
    public class CNCMachine
    {
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
            while(_autoCheckConnet)
            {
                Thread.Sleep(500);
            }
        }
    }
}
