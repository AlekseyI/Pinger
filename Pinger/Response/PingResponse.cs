using Pinger.Enums;
using System;

namespace Pinger.Response
{
    public class PingResponse : IPingResponse
    {
        public DateTime Time { get; }
        public TypeProtocol Protocol { get; }
        public string Host { get; }
        public PingStatus Status { get; }

        public PingResponse(DateTime time, TypeProtocol protocol, string host, PingStatus status)
        {
            Time = time;
            Protocol = protocol;
            Host = host;
            Status = status;
        }

        public override string ToString()
        {
            return $"{ Time } { Protocol } { Host } { Status }";
        }
    }
}