using Pinger.Enums;

namespace Pinger.Config
{
    public class ConfigSource : IConfigSource
    {
        public ConfigFormat Format { get; }
        public string Path { get; }

        public ConfigSource(string path, ConfigFormat format)
        {
            Path = path;
            Format = format;
        }
    }
}