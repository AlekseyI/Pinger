using System.Threading.Tasks;

namespace Pinger.Log
{
    public interface ILog
    {
        ILogInput LogInput { get; }
        void Write(ILogData logData);
        Task WriteAsync(ILogData logData);
    }
}