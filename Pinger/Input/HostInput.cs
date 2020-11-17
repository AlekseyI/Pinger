using System;

namespace Pinger.Input
{
    public class HostInput : IHostInput
    {
        public string Address { get; }
        public TimeSpan TimeOut { get; }

        public HostInput() { }

        public HostInput(string address, TimeSpan timeOut)
        {
            Address = address;
            TimeOut = timeOut;
        }
    }
}