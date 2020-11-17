using Pinger.Config;
using Pinger.Config.JsonFile;
using Pinger.Enums;
using Pinger.Exceptions;
using System;

namespace Pinger.Factory
{
    public class ConfigFactory : IFactory<IConfigInput, IConfig>
    {
        public IConfig GetInstance(IConfigInput configInput)
        {
            if (configInput == null)
            {
                throw new ConfigException(nameof(configInput), nameof(ArgumentNullException));
            }

            switch(configInput.Format)
            {
                case ConfigFormatEnum.JsonFile:
                    return new ConfigJsonFile<ConfigJsonData>(configInput);
                default:
                    throw new ConfigException($"{nameof(configInput.Format)} = {configInput.Format} is unsupported", nameof(ArgumentException));
            }
        }
    }
}
