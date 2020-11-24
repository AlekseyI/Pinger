using Pinger.Config;
using System;

namespace Pinger.Manager
{
    public interface IPingManager : IDisposable
    {
        void Start();
        void Stop();
        bool CheckConfig(IConfigData configData);
    }
}
