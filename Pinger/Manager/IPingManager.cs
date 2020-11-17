using System;
using System.Threading.Tasks;

namespace Pinger.Manager
{
    public interface IPingManager : IDisposable
    {
        void Start();
        void Stop();
        bool CheckConfig();
    }
}
