using Pinger.Enums;
using System;

namespace Pinger.Response
{
    public interface IPingResponse : IPingResponseFormat
    {
        DateTime Time { get; }
        ProtocolTypeEnum Protocol { get; }
        string Host { get; } 
        PingStatusEnum Status { get; } 
    }
}
