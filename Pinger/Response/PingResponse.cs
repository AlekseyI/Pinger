using Pinger.Enums;
using System;

namespace Pinger.Response
{
    public class PingResponse : IPingResponse
    {
        public DateTime Time { get; }
        public ProtocolTypeEnum Protocol { get; }
        public string Host { get; }
        public PingStatusEnum Status { get; }

        public PingResponse(DateTime time, ProtocolTypeEnum protocol, string host, PingStatusEnum status)
        {
            Time = time;
            Protocol = protocol;
            Host = host;
            Status = status;
        }

        public PingResponse() { }

        public string FormatToString()
        {
            return $"{ Time } { Protocol } { Host } { Status }";
        }
    }
}