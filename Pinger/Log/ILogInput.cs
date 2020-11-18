using Pinger.Enums;

namespace Pinger.Log
{
    public interface ILogInput
    {
        LogFormatEnum Format { get; }
        string Path { get; }
    }
}