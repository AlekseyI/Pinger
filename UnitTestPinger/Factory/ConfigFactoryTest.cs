using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger;
using Pinger.Config;
using Pinger.Config.JsonFile;
using Pinger.Enums;
using Pinger.Factory;
using System;
using System.Collections.Generic;

namespace UnitTestPinger.Factory
{
    [TestClass]
    public class ConfigFactoryTest
    {
        [TestMethod]
        public void ConfigFactoryGetConfigJosnFileInstanceSuccessTest()
        {
            Assert.IsInstanceOfType(new ConfigFactory().GetInstance(new ConfigSource(Constant.Config, ConfigFormat.JsonFile)), typeof(ConfigJsonFile<IEnumerable<ConfigJsonData>>));
        }

        [TestMethod]
        public void ConfigFactoryConfigFormatNullTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConfigFactory().GetInstance(null));
        }

        [TestMethod]
        public void ConfigFactoryConfigFormatUnsupportedTest()
        {
            var factory = new ConfigOtherFactory(ConfigOtherFactory.ConfigFormatEnum.Unknown);

            Assert.ThrowsException<ArgumentException>(() => factory.GetInstance(new ConfigSource("", ConfigFormat.JsonFile)));
        }

        public class ConfigOtherFactory : IFactory<IConfigSource, IConfig<IConfigSource, IEnumerable<IConfigData>>>
        {
            public enum ConfigFormatEnum
            {
                JsonFile,
                Unknown = 1000
            }

            private ConfigFormatEnum _configFormat;

            public ConfigOtherFactory(ConfigFormatEnum configFormat)
            {
                _configFormat = configFormat;
            }

            public IConfig<IConfigSource, IEnumerable<IConfigData>> GetInstance(IConfigSource configSource)
            {
                switch (_configFormat)
                {
                    case ConfigFormatEnum.JsonFile:
                        return new ConfigJsonFile<IEnumerable<ConfigJsonData>>(configSource);
                    default:
                        throw new ArgumentException($"{nameof(configSource.Format)} = {_configFormat} is unsupported");
                }
            }
        }
    }
}
