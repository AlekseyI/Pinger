using System;
using System.Data;

namespace Pinger.Exceptions
{
    public class LogException : Exception
    {
        public string NameException { get; }

        public LogException() { }
        public LogException(string message) : base(message) { }
        public LogException(string message, string nameException) : base(message)
        {
            NameException = nameException;
        }

        public override string ToString()
        {
            return $"{nameof(LogException)} -> {NameException} : {base.Message}";
        }
    }
}
