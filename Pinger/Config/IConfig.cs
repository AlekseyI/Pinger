using System.Collections.Generic;

namespace Pinger.Config
{
    public interface IConfig
    {
        IConfigInput ConfigInput { get; }
        IEnumerable<IConfigData> Read();
        void Write(IEnumerable<IConfigData> configData);
        bool CreateDefaultConfig(IConfigData configData);
    }
}
