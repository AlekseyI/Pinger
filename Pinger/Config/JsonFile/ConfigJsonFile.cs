using Newtonsoft.Json;
using Pinger.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pinger.Config.JsonFile
{
    public class ConfigJsonFile<T> : IConfig where T : ConfigJsonData
    {
        public IConfigInput ConfigInput { get; }

        public ConfigJsonFile(IConfigInput configInput)
        {
            if (configInput == null)
            {
                throw new ConfigException(nameof(configInput), nameof(ArgumentNullException));
            }
            else if (string.IsNullOrEmpty(configInput.Path))
            {
                throw new ConfigException(nameof(configInput.Path) + " is null or empty", nameof(ArgumentException));
            }
            else
            {
                ConfigInput = configInput;
            }
        }

        public IEnumerable<IConfigData> Read()
        {
            IEnumerable<T> configData = null;
            StreamReader stream = null;

            try
            {
                using (stream = new StreamReader(ConfigInput.Path))
                {
                    configData = JsonConvert.DeserializeObject<IEnumerable<T>>(stream.ReadToEnd());
                }
            }
            catch (IOException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
            catch (JsonSerializationException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
            catch (ArgumentException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
            catch(SystemException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }

            return configData;
        }

        public void Write(IEnumerable<IConfigData> configData)
        {
            StreamWriter stream = null;

            try
            {
                using (stream = new StreamWriter(ConfigInput.Path))
                {
                    stream.Write(JsonConvert.SerializeObject(configData, Formatting.Indented));
                }
            }
            catch (IOException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
            catch (JsonSerializationException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
            catch (ArgumentException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
            catch (SystemException e)
            {
                stream?.Close();
                throw new ConfigException(e.Message, e.GetType().Name);
            }
        }

        public bool CreateDefaultConfig(IConfigData configData)
        {
            if (!File.Exists(ConfigInput.Path))
            {
                Write(new IConfigData[] { configData });
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}