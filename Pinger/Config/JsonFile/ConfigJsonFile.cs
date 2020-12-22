using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pinger.Config.JsonFile
{
    public class ConfigJsonFile<T> : IConfig<IConfigSource, IEnumerable<IConfigData>> where T : IEnumerable<ConfigJsonData>
    {
        public IConfigSource ConfigSource { get; }

        public ConfigJsonFile(IConfigSource configSource)
        {
            if (configSource == null)
            {
                throw new ArgumentNullException(nameof(configSource));
            }
            else if (string.IsNullOrEmpty(configSource.Path))
            {
                throw new ArgumentException(nameof(configSource.Path) + " is null or empty");
            }
            else
            {
                ConfigSource = configSource;
            }
        }

        public IEnumerable<IConfigData> Read()
        {
            using (var stream = new StreamReader(ConfigSource.Path))
            {
                return JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
            }
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
            if (File.Exists(ConfigSource.Path))
            {
                return false;
            }

            Write(configData);
            return true;
        }
    }
}
