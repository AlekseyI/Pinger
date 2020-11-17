using Pinger.Enums;

namespace Pinger.Config
{
    public interface IConfigInput
    {
        ConfigFormatEnum Format { get; }
        string Path { get; }
    }
}
