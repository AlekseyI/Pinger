using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinger.Input
{
    public class HostPortInput : IHostPortInput
    {
        public int Port { get; }

        public string Address { get; }

        public TimeSpan TimeOut { get; }

        public HostPortInput(string address, int port, TimeSpan timeOut)
        {
            Address = address;
            Port = port;
            TimeOut = timeOut;
        }
    }
}
