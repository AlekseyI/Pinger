using System;

namespace Pinger.Exceptions
{
    public class PingRequestException : Exception
    {
        public string NameException { get; }

        public PingRequestException() { }
        public PingRequestException(string message) : base(message) { }

        public PingRequestException(string message, string nameException) : base(message)
        {
            NameException = nameException;
        }

        public override string ToString()
        {
            return $"{nameof(PingRequestException)} -> {NameException} : {base.Message}";
        }
    }
}
