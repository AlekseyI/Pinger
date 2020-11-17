using System;
using Pinger.Enums;

namespace Pinger.Config
{
    public interface IConfigData
    {
        string Host { get; set; }
        TimeSpan Period { get; set; }
        TimeSpan TimeOut { get; set; }
        ProtocolTypeEnum Protocol { get; set; }
    }
}