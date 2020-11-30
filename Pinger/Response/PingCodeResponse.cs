using Pinger.Enums;
using System;

namespace Pinger.Response
{
    public class PingCodeResponse : IPingCodeResponse
    {
        public DateTime Time { get; }
        public TypeProtocol Protocol { get; }
        public string Host { get; }
        public PingStatus Status { get; }
        public int Code { get; }

        public PingCodeResponse(DateTime time, TypeProtocol protocol, string host, PingStatus status, int code)
        {
            Time = time;
            Protocol = protocol;
            Host = host;
            Status = status;
            Code = code;
        }

        public PingCodeResponse() { }

        public string FormatToString()
        {
            return $"{ Time } { Protocol } { Host } { Status } { Code }";
        }
    }
}