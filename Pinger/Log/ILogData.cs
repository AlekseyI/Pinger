using Pinger.Response;

namespace Pinger.Log
{
    public interface ILogData
    {
        IPingResponse Log { get; set; }
    }
}