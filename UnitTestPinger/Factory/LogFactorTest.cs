using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger;
using Pinger.Enums;
using Pinger.Factory;
using Pinger.Logger;
using Pinger.Logger.TextFile;
using System;

namespace UnitTestPinger.Factory
{
    [TestClass]
    public class LogFactorTest
    {
        [TestMethod]
        public void LogFactoryGetLogTextFileInstanceSuccessTest()
        {
            Assert.IsInstanceOfType(new LogFactory().GetInstance(new LogSource(Constant.Log, LogFormat.TextFile)), typeof(LoggerTextFile));
        }

        [TestMethod]
        public void LogFactoryLogFormatNullTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogFactory().GetInstance(null));
        }

        [TestMethod]
        public void LogFactoryLogFormatUnsupportedTest()
        {
            var factory = new LogOtherFactory(LogOtherFactory.LogFormatEnum.Unknown);

            Assert.ThrowsException<ArgumentException>(
                () => factory.GetInstance(new LogSource("", LogFormat.TextFile)));
        }

        public class LogOtherFactory : IFactory<ILogSource, ILogger<ILogSource, ILogData>>
        {
            public enum LogFormatEnum
            {
                TextFile,
                Unknown = 1000
            }

            private LogFormatEnum _logFormat;

            public LogOtherFactory(LogFormatEnum logFormat)
            {
                _logFormat = logFormat;
            }

            public ILogger<ILogSource, ILogData> GetInstance(ILogSource logSource)
            {
                switch (_logFormat)
                {
                    case LogFormatEnum.TextFile:
                        return new LoggerTextFile(logSource);
                    default:
                        throw new ArgumentException($"{nameof(logSource.Format)} = {_logFormat} is unsupported");
                }
            }
        }
    }
}
