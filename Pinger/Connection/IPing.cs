using System;
using System.Threading.Tasks;

namespace Pinger.Connection
{
    public interface IPing<out T, out U> : IDisposable
    {
        T HostInput { get; }
        U Response { get; }
        Task Ping();
    }
}