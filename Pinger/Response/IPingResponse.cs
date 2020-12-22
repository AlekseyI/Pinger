using Pinger.Enums;
using System;

namespace Pinger.Response
{
    public interface IPingResponse
    {
        DateTime Time { get; }
        TypeProtocol Protocol { get; }
        string Host { get; } 
        PingStatus Status { get; } 
    }
}
