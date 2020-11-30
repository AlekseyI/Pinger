using Pinger.Response;

namespace Pinger.Logger
{
    public class LogData : ILogData
    {
        public IPingResponse Log { get; set; }

        public LogData() { }

        public LogData(IPingResponse log)
        {
            Log = log;
        }
    }
}