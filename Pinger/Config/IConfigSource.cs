using Pinger.Enums;

namespace Pinger.Config
{
    public interface IConfigSource
    {
        ConfigFormat Format { get; }
        string Path { get; }
    }
}
