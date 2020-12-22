using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pinger;
using Pinger.Enums;
using Pinger.Logger;
using Pinger.Logger.TextFile;
using System;

namespace UnitTestPinger
{
    [TestClass]
    public class LogTexFileTest
    {
        [TestMethod]
        public void LogTextFileLogInputNullFailTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LoggerTextFile(null));
        }

        [TestMethod]
        public void LogTextFileLogDataNullFailTest()
        {
            var logger = new Mock<ILogger<ILogSource, ILogData>>();
            logger.Setup(v => v.WriteAsync(null)).Throws<ArgumentNullException>();

            Assert.ThrowsException<ArgumentNullException>(() => logger.Object.WriteAsync(null).Wait());
        }
    }
}
