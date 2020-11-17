using System;

namespace Pinger.Input
{
    public interface IHostInput
    {
        string Address { get; }
        TimeSpan TimeOut { get; }
    }
}
