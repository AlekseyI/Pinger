using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger;
using Pinger.Config;
using Pinger.Config.JsonFile;
using Pinger.Enums;
using Pinger.Factory;
using System;

namespace UnitTestPinger.Factory
{
    [TestClass]
    public class DefaultConfigFactoryTest
    {
        [TestMethod]
        public void DefaultConfigFactoryGetConfigJosnDataInstanceSuccessTest()
        {
            Assert.IsInstanceOfType(new DefaultConfigFactory().GetInstance(new ConfigSource(Constant.Config, ConfigFormat.JsonFile)), typeof(ConfigJsonData));
        }

        [TestMethod]
        public void DefaultConfigFactoryConfigFormatNullTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DefaultConfigFactory().GetInstance(null));
        }

        [TestMethod]
        public void DefaultConfigFactoryConfigFormatUnsupportedTest()
        {
            var factory = new DefaultConfigOtherFactory(DefaultConfigOtherFactory.ConfigFormatEnum.Unknown);

            Assert.ThrowsException<ArgumentException>(() => factory.GetInstance(new ConfigSource("", ConfigFormat.JsonFile)));
        }

        public class DefaultConfigOtherFactory : IFactory<IConfigSource, IConfigData>
        {
            public enum ConfigFormatEnum
            {
                JsonFile,
                Unknown = 1000
            }

            private ConfigFormatEnum _configFormat;

            public DefaultConfigOtherFactory(ConfigFormatEnum configFormat)
            {
                _configFormat = configFormat;
            }

            public IConfigData GetInstance(IConfigSource configSource)
            {
                switch (_configFormat)
                {
                    case ConfigFormatEnum.JsonFile:
                        return new ConfigJsonData();
                    default:
                        throw new ArgumentException($"{nameof(configSource.Format)} = {_configFormat} is unsupported");
                }
            }
        }
    }
}
