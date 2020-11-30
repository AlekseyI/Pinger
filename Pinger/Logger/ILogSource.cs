using Pinger.Enums;

namespace Pinger.Logger
{
    public interface ILogSource
    {
        LogFormat Format { get; }
        string Path { get; }
    }
}