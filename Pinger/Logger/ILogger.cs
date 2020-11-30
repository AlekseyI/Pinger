using System.Threading.Tasks;

namespace Pinger.Logger
{
    public interface ILogger<T, U>
    {
        T LoggerSource { get; }
        Task WriteAsync(U loggerData);
    }
}