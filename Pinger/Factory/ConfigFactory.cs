using Pinger.Config;
using Pinger.Config.JsonFile;
using Pinger.Enums;
using System;
using System.Collections.Generic;

namespace Pinger.Factory
{
    public class ConfigFactory : IFactory<IConfigSource, IConfig<IConfigSource, IEnumerable<IConfigData>>>
    {
        public IConfig<IConfigSource, IEnumerable<IConfigData>> GetInstance(IConfigSource configSource)
        {
            if (configSource == null)
            {
                throw new ArgumentNullException(nameof(configSource));
            }

            switch(configSource.Format)
            {
                case ConfigFormat.JsonFile:
                    return new ConfigJsonFile<IEnumerable<ConfigJsonData>>(configSource);
                default:
                    throw new ArgumentException($"{nameof(configSource.Format)} = {configSource.Format} is unsupported");
            }
        }
    }
}
