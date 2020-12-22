using System.Threading.Tasks;

namespace Pinger.Logger
{
    public interface ILogger<T, U>
    {
        T LogSource { get; }
        Task WriteAsync(U logData);
    }
}