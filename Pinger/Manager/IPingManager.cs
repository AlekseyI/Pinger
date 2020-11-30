using Pinger.Config;
using System;

namespace Pinger.Manager
{
    public interface IPingManager : IDisposable
    {
        void Start(bool isWait);
        void Stop();
        bool CheckConfig(IConfigData configData);
    }
}
