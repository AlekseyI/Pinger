using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pinger.Config;
using Pinger.Connection;
using Pinger.Factory;
using Pinger.Input;
using Pinger.Logger;
using Pinger.Manager;
using Pinger.Response;
using System;
using System.Collections.Generic;

namespace UnitTestPinger
{
    [TestClass]
    public class PingManagerTest
    {
        [TestMethod]
        public void PingManagerCreateInstanceSuccessTest()
        {
            var pingRequestFactory = Mock.Of<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();
            var config = Mock.Of<IConfig<IConfigSource, IEnumerable<IConfigData>>>();
            var log = Mock.Of<ILogger<ILogSource, ILogData>>();

            var pingManager = new PingManager(pingRequestFactory, config, log);

            Assert.IsInstanceOfType(pingManager, typeof(PingManager));
        }

        [TestMethod]
        public void PingManagerNullArgumentsFailTest()
        {
            var pingRequestFactory = Mock.Of<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();
            var config = Mock.Of<IConfig<IConfigSource, IEnumerable<IConfigData>>>();
            var log = Mock.Of<ILogger<ILogSource, ILogData>>();

            Assert.ThrowsException<ArgumentNullException>(() => new PingManager(null, config, log));
            Assert.ThrowsException<ArgumentNullException>(() => new PingManager(pingRequestFactory, null, log));
            Assert.ThrowsException<ArgumentNullException>(() => new PingManager(pingRequestFactory, config, null));
        }


        [TestMethod]
        public void PingManagerCheckConfigNullArgumentFailTest()
        {
            var pingRequestFactory = Mock.Of<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();
            var config = Mock.Of<IConfig<IConfigSource, IEnumerable<IConfigData>>>();
            var log = Mock.Of<ILogger<ILogSource, ILogData>>();

            var pingManager = new PingManager(pingRequestFactory, config, log);

            Assert.ThrowsException<ArgumentNullException>(() => pingManager.CheckConfig(null));
        }
    }
}
