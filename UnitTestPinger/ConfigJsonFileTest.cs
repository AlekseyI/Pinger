using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger.Config;
using Pinger.Config.JsonFile;
using System;
using Pinger.Enums;
using Moq;
using System.Collections.Generic;

namespace UnitTestPinger
{
    [TestClass]
    public class ConfigJsonFileTest
    {
        [TestMethod]
        public void ConfigJsonFileConfigSourceNullFailTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConfigJsonFile<ConfigJsonData[]>(null));
        }

        [TestMethod]
        public void ConfigJsonFileConfigSourcePathNullFailTest()
        {
            var configSource = new ConfigSource(null, ConfigFormat.JsonFile);
            Assert.ThrowsException<ArgumentException>(() => new ConfigJsonFile<ConfigJsonData[]>(configSource));
        }

        [TestMethod]
        public void ConfigJsonFileConfigDataNullFailTest()
        {
            var config = new Mock<IConfig<IConfigSource, IEnumerable<IConfigData>>>();
            config.Setup(v => v.Write(null)).Throws<ArgumentNullException>();

            Assert.ThrowsException<ArgumentNullException>(() => config.Object.Write(null));
        }
    }
}
