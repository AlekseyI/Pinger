using Pinger.Response;

namespace Pinger.Log
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