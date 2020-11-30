using Pinger.Response;

namespace Pinger.Logger
{
    public interface ILogData
    {
        IPingResponse Log { get; set; }
    }
}