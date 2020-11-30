using Pinger.Enums;

namespace Pinger.Logger
{
    public class LogSource : ILogSource
    {
        public LogFormat Format { get; }
        public string Path { get; }

        public LogSource() { }

        public LogSource(string path, LogFormat format)
        {
            Path = path;
            Format = format;
        }
    }
}