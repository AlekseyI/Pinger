using Pinger.Enums;
using System;

namespace Pinger.Response
{
    public class PingCodeResponse : IPingCodeResponse
    {
        public DateTime Time { get; }
        public ProtocolTypeEnum Protocol { get; }
        public string Host { get; }
        public PingStatusEnum Status { get; }
        public int Code { get; }

        public PingCodeResponse(DateTime time, ProtocolTypeEnum protocol, string host, PingStatusEnum status, int code)
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