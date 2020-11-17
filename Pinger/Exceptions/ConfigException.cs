using System;

namespace Pinger.Exceptions
{
    public class ConfigException : Exception
    {
        public string NameException { get; }

        public ConfigException() { }

        public ConfigException(string message) : base(message) { }
        public ConfigException(string message, string nameException) : base(message)
        {
            NameException = nameException;
        }

        public override string ToString()
        {
            return $"{nameof(ConfigException)} -> {NameException} : {base.Message}";
        }
    }
}