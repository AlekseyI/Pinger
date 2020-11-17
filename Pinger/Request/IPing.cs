using Pinger.Input;
using Pinger.Response;
using System;

namespace Pinger.Request
{
    public interface IPing<out T, out U> : IDisposable where T : IHostInput where U : IPingResponse
    {
        T HostInput { get; }
        U Ping();
    }
}