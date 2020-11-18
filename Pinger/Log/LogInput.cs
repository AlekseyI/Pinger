using Pinger.Enums;

namespace Pinger.Log
{
    public class LogInput : ILogInput
    {
        public LogFormatEnum Format { get; }
        public string Path { get; }

        public LogInput() { }

        public LogInput(string path, LogFormatEnum format)
        {
            Path = path;
            Format = format;
        }
    }
}