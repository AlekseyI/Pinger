using Pinger.Config;
using Pinger.Config.JsonFile;
using Pinger.Enums;
using System;

namespace Pinger.Factory
{
    public class DefaultConfigFactory : IFactory<IConfigSource, IConfigData>
    {
        public IConfigData GetInstance(IConfigSource configSource)
        {
            if (configSource == null)
            {
                throw new ArgumentNullException(nameof(configSource));
            }

            switch (configSource.Format)
            {
                case ConfigFormat.JsonFile:
                    return new ConfigJsonData();
                default:
                    throw new ArgumentException($"{nameof(configSource.Format)} = {configSource.Format} is unsupported");
            }
        }
    }
}
