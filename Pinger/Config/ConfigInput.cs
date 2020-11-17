using Pinger.Enums;

namespace Pinger.Config
{
    public class ConfigInput : IConfigInput
    {
        public ConfigFormatEnum Format { get; }
        public string Path { get; }

        public ConfigInput() { }

        public ConfigInput(string path, ConfigFormatEnum format)
        {
            Path = path;
            Format = format;
        }
    }
}