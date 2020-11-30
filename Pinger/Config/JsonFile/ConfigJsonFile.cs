using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pinger.Config.JsonFile
{
    public class ConfigJsonFile<T> : IConfig<IConfigSource, IEnumerable<IConfigData>> where T : IEnumerable<ConfigJsonData>
    {
        public IConfigSource ConfigSource { get; }

        public ConfigJsonFile(IConfigSource configInput)
        {
            if (configInput == null)
            {
                throw new ArgumentNullException(nameof(configInput));
            }
            else if (string.IsNullOrEmpty(configInput.Path))
            {
                throw new ArgumentException(nameof(configInput.Path) + " is null or empty");
            }
            else
            {
                ConfigSource = configInput;
            }
        }

        public IEnumerable<IConfigData> Read()
        {
            T configData;

            using (var stream = new StreamReader(ConfigSource.Path))
            {
                configData = JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
                
            }

            return configData;
        }

        public void Write(IEnumerable<IConfigData> configData)
        {
            using (var stream = new StreamWriter(ConfigSource.Path))
            {
                stream.Write(JsonConvert.SerializeObject(configData, Formatting.Indented));
            }
        }

        public bool CreateDefaultConfig(IEnumerable<IConfigData> configData)
        {
            if (!File.Exists(ConfigSource.Path))
            {
                Write(configData);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}