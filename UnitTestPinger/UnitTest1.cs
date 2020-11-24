using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Pinger.Request;
using Pinger.Input;
using Pinger.Enums;
using System;
using Pinger.Exceptions;
using Moq;
using Pinger.Response;
using Pinger;
using Pinger.Config;
using Pinger.Factory.Ping;
using Pinger.Factory;
using Pinger.Log;
using Pinger.Log.TextFile;
using Pinger.Config.JsonFile;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pinger.Manager;

namespace UnitTestPinger
{
    [TestClass]
    public class UnitTest1
    {

        #region TcpRequest

        private string _serverOutput;
        private string _ip = "127.0.0.1";
        private int _port = 11000;

        [TestMethod]
        public void TcpPingSuccessTest()
        {
            using (var server = new TestTcpServer(5))
            {
                var hostInput = new HostPortInput(_ip, _port, new TimeSpan(0, 0, 5));

                server.EventStatus += EventHandler;
                server.Start(hostInput.Port);

                Thread.Sleep(1000);

                Assert.AreEqual(TestTcpServer.ServerWaitStatus, _serverOutput);

                using (var tcpRequest = new TcpRequest(hostInput))
                {
                    var response = tcpRequest.Ping();
                    Assert.IsTrue(response.Host == hostInput.Address && response.Protocol == ProtocolTypeEnum.Tcp && response.Status == PingStatusEnum.Ok && response.Code == 0);
                }
            }
            _serverOutput = null;
        }

        [TestMethod]
        public void TcpPingInvalidHostFormatFailTest()
        {
            var hostInput = new HostPortInput("127.0.0", _port, new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(FormatException)).ToString());
        }

        [TestMethod]
        public void TcpPingTimeOutFailTest()
        {
            var tcpRequestMock = new Mock<IPing<IHostPortInput, IPingCodeResponse>>();
            var expected = new PingCodeResponse(DateTime.Now, ProtocolTypeEnum.Tcp, _ip, PingStatusEnum.TimeOut, -1);

            tcpRequestMock.Setup(v => v.Ping()).Returns(expected);

            var response = tcpRequestMock.Object.Ping();
            
            Assert.AreEqual(expected.FormatToString(), response.FormatToString());
        }

        [TestMethod]
        public void TcpPingHostInputNullFailTest()
        {
            IHostPortInput hostPort = null;
            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostPort),
                new PingRequestException(nameof(hostPort), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void TcpPingHostInputAddressNullOrEmptyFailTest()
        {
            var hostInput = new HostPortInput(null, _port, new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException)).ToString());

            hostInput = new HostPortInput("", _port, new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException)).ToString());
        }

        [TestMethod]
        public void TcpPingTimeOutOverFlowFailTest()
        {
            var hostInput = new HostPortInput(_ip, _port, TimeSpan.Zero);

            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());

            hostInput = new HostPortInput(_ip, _port, new TimeSpan(0, 0, -1));

            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());

            hostInput = new HostPortInput(_ip, _port, new TimeSpan(25, 0, 0, 0, 0));

            Assert.ThrowsException<PingRequestException>(
                () => new TcpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());
        }

        [TestMethod]
        public void TcpPingOtherExceptionsFailTest()
        {
            var icmpRequest = new Mock<IPing<IHostPortInput, IPingCodeResponse>>();

            icmpRequest.Setup(v => v.Ping()).Throws<PingRequestException>();

            Assert.ThrowsException<PingRequestException>(
                () => icmpRequest.Object.Ping());
        }

        private void EventHandler(string status)
        {
            _serverOutput = status;
        }

        #endregion

        #region IcmpRequest

        [TestMethod]
        public void IcmpPingSuccessTest()
        {
            var hostInput = new HostInput("ya.ru", new TimeSpan(0, 0, 5));

            using (var icmpRequest = new IcmpRequest(hostInput))
            {
                var response = icmpRequest.Ping();
                Assert.IsTrue(response.Host == hostInput.Address && response.Protocol == ProtocolTypeEnum.Icmp && response.Status == PingStatusEnum.Ok);
            }
        }

        [TestMethod]
        public void IcmpPingInvalidHostFormatFailTest()
        {
            var hostInput = new HostInput("ya", new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(FormatException)).ToString());
        }

        [TestMethod]
        public void IcmpPingTimeOutFailTest()
        {
            var hostInput = new HostInput("10.255.255.1", new TimeSpan(0, 0, 1));

            using (var icmpRequest = new IcmpRequest(hostInput))
            {
                var response = icmpRequest.Ping();
                Assert.IsTrue(response.Host == hostInput.Address && response.Protocol == ProtocolTypeEnum.Icmp && response.Status == PingStatusEnum.TimeOut);
            }
        }

        [TestMethod]
        public void IcmpPingHostInputNullFailTest()
        {
            IHostInput hostInput = null;

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void IcmpPingHostInputAddressNullOrEmptyFailTest()
        {
            var hostInput = new HostInput(null, new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException)).ToString());

            hostInput = new HostInput("", new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException)).ToString());
        }

        [TestMethod]
        public void IcmpPingTimeOutOverFlowFailTest()
        {
            var hostInput = new HostInput("ya.ru", TimeSpan.Zero);

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());

            hostInput = new HostInput("ya.ru", new TimeSpan(0, 0, -1));

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());

            hostInput = new HostInput("ya.ru", new TimeSpan(25, 0, 0, 0, 0));

            Assert.ThrowsException<PingRequestException>(
                () => new IcmpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());
        }

        [TestMethod]
        public void IcmpPingOtherExceptionsFailTest()
        {
            var icmpRequest = new Mock<IPing<IHostInput, IPingResponse>>();

            icmpRequest.Setup(v => v.Ping()).Throws<PingRequestException>();

            Assert.ThrowsException<PingRequestException>(
                () => icmpRequest.Object.Ping());
        }

        #endregion

        #region HttpRequest

        [TestMethod]
        public void HttpPingSuccessTest()
        {
            var hostInput = new HostInput("https://ya.ru", new TimeSpan(0, 0, 5));

            using (var httpRequest = new HttpRequest(hostInput))
            {
                var response = httpRequest.Ping();
                Assert.IsTrue(response.Host == hostInput.Address && response.Protocol == ProtocolTypeEnum.Http && response.Status == PingStatusEnum.Ok && response.Code == 200);
            }
        }

        [TestMethod]
        public void HttpPingInvalidHostFormatFailTest()
        {
            var hostInput = new HostInput("https:ya.ru", new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(FormatException)).ToString());
        }

        [TestMethod]
        public void HttpPingTimeOutFailTest()
        {
            var hostInput = new HostInput("https://google.com:81", new TimeSpan(0, 0, 1));

            using (var httpRequest = new HttpRequest(hostInput))
            {
                var response = httpRequest.Ping();
                Assert.IsTrue(response.Host == hostInput.Address && response.Protocol == ProtocolTypeEnum.Http && response.Status == PingStatusEnum.TimeOut && response.Code == -1);
            }
        }

        [TestMethod]
        public void HttpPingHostInputNullFailTest()
        {
            IHostInput hostInput = null;

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void HttpPingHostInputAddressNullOrEmptyFailTest()
        {
            var hostInput = new HostInput(null, new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException)).ToString());

            hostInput = new HostInput("", new TimeSpan(0, 0, 5));

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException)).ToString());
        }

        [TestMethod]
        public void HttpPingTimeOutOverFlowFailTest()
        {
            var hostInput = new HostInput("https://ya.ru", TimeSpan.Zero);

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());

            hostInput = new HostInput("https://ya.ru", new TimeSpan(0, 0, -1));

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());

            hostInput = new HostInput("https://ya.ru", new TimeSpan(25, 0, 0, 0, 0));

            Assert.ThrowsException<PingRequestException>(
                () => new HttpRequest(hostInput),
                new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException)).ToString());
        }

        [TestMethod]
        public void HttpPingOtherExceptionsFailTest()
        {
            var httpRequest = new Mock<IPing<IHostInput, IPingCodeResponse>>();

            httpRequest.Setup(v => v.Ping()).Throws<PingRequestException>();

            Assert.ThrowsException<PingRequestException>(
                () => httpRequest.Object.Ping());
        }

        #endregion

        #region HostInputValidate

        [TestMethod]
        public void HttpHostInputCheckSuccessTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.IsTrue(hostInputCheck.Check("https://google.com", Constant.HttpUrlOrIpCheckPattern));

            Assert.IsTrue(hostInputCheck.Check("https://google.com:80", Constant.HttpUrlOrIpCheckPattern));

            Assert.IsTrue(hostInputCheck.Check("https://173.194.216.113", Constant.HttpUrlOrIpCheckPattern));

            Assert.IsTrue(hostInputCheck.Check("https://173.194.216.113:80", Constant.HttpUrlOrIpCheckPattern));
        }

        [TestMethod]
        public void HttpHostInputCheckFormatFailTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.IsFalse(hostInputCheck.Check("https:google.com", Constant.HttpUrlOrIpCheckPattern));

            Assert.IsFalse(hostInputCheck.Check("https://google.com:", Constant.HttpUrlOrIpCheckPattern));

            Assert.IsFalse(hostInputCheck.Check("https://173.194.216", Constant.HttpUrlOrIpCheckPattern));

            Assert.IsFalse(hostInputCheck.Check("asd", Constant.HttpUrlOrIpCheckPattern));
        }

        [TestMethod]
        public void IcmpHostInputSuccessTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.IsTrue(hostInputCheck.Check("google.com", Constant.IcmpUrlOrIpCheckPattern));

            Assert.IsTrue(hostInputCheck.Check("173.194.216.113", Constant.IcmpUrlOrIpCheckPattern));
        }

        [TestMethod]
        public void IcmpHostInputCheckFormatFailTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.IsFalse(hostInputCheck.Check("google", Constant.IcmpUrlOrIpCheckPattern));

            Assert.IsFalse(hostInputCheck.Check("173.194.216", Constant.IcmpUrlOrIpCheckPattern));
        }

        [TestMethod]
        public void TcpHostInputSuccessTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.IsTrue(hostInputCheck.Check("173.194.216.113", Constant.TcpIpCheckPattern));
        }

        [TestMethod]
        public void TcpHostInputCheckFormatFailTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.IsFalse(hostInputCheck.Check("173.194.216", Constant.TcpIpCheckPattern));
        }

        [TestMethod]
        public void HostInputCheckHostOrPatternNullFailTest()
        {
            var hostInputCheck = new HostInputValidate();
            string host = null;
            string formatPattern = null;

            Assert.ThrowsException<PingRequestException>(
                () => hostInputCheck.Check(host, Constant.HttpUrlOrIpCheckPattern),
                new PingRequestException(nameof(host), nameof(ArgumentException)).ToString());

            Assert.ThrowsException<PingRequestException>(
                () => hostInputCheck.Check("https://ya.ru", formatPattern),
                new PingRequestException(nameof(formatPattern), nameof(ArgumentException)).ToString());
        }

        [TestMethod]
        public void TcpHostInputParseSuccessTest()
        {
            var hostInputCheck = new HostInputValidate();

            (var ip, var port) = hostInputCheck.Parse("173.194.216.113:1234");

            Assert.AreEqual(ip, "173.194.216.113");
            Assert.AreEqual(port, 1234);
        }

        [TestMethod]
        public void TcpHostInputParseFormatFailTest()
        {
            var hostInputCheck = new HostInputValidate();
            string host = null;

            Assert.ThrowsException<PingRequestException>(
                () => hostInputCheck.Parse("173.194.216.113"),
                new PingRequestException(nameof(host), nameof(FormatException)).ToString());

            Assert.ThrowsException<PingRequestException>(
                () => hostInputCheck.Parse("173.194.216.113:" + long.MaxValue.ToString()),
                new PingRequestException(nameof(host), nameof(OverflowException)).ToString());
        }

        [TestMethod]
        public void HostInputParseHostNullFailTest()
        {
            var hostInputCheck = new HostInputValidate();
            string host = null;

            Assert.ThrowsException<PingRequestException>(
                () => hostInputCheck.Parse(host),
                new PingRequestException(nameof(host), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void HostInputCheckOtherExceptionsFailTest()
        {
            var hostInputCheckMock = new Mock<IHostInputCheck>();

            hostInputCheckMock.Setup(v => v.Check("ya.ru", Constant.IcmpUrlOrIpCheckPattern)).Throws<PingRequestException>();

            Assert.ThrowsException<PingRequestException>(
                () => hostInputCheckMock.Object.Check("ya.ru", Constant.IcmpUrlOrIpCheckPattern));
        }

        [TestMethod]
        public void HostInputCheckParseExceptionsFailTest()
        {
            var hostInputParseMock = new Mock<IHostInputParse>();

            hostInputParseMock.Setup(v => v.Parse("127.0.0.1:80")).Throws<PingRequestException>();

            Assert.ThrowsException<PingRequestException>(
                () => hostInputParseMock.Object.Parse("127.0.0.1:80"));
        }

        #endregion

        #region LogTexFile

        [TestMethod]
        public void LogTextFileWriteSuccessTest()
        {
            var pingResponse = new PingResponse(DateTime.Now, ProtocolTypeEnum.Icmp, "ya.ru", PingStatusEnum.Ok);

            var logData = new LogData(pingResponse);

            var loggerMock = new Mock<ILog>();

            loggerMock.Setup(v => v.Write(logData));

            loggerMock.Object.Write(logData);
        }

        [TestMethod]
        public void LogTextFileWriteAsyncSuccessTest()
        {
            var pingResponse = new PingResponse(DateTime.Now, ProtocolTypeEnum.Icmp, "ya.ru", PingStatusEnum.Ok);

            var logData = new LogData(pingResponse);

            var loggerMock = new Mock<ILog>();

            loggerMock.Setup(v => v.WriteAsync(logData));

            loggerMock.Object.WriteAsync(logData);
        }

        [TestMethod]
        public void LogTextFileLogInputNullFailTest()
        {
            ILogInput logInput = null;

            Assert.ThrowsException<LogException>(
                () => new LogTextFile(logInput),
                new LogException(nameof(logInput), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void LogTextFileLogDataNullFailTest()
        {
            var logInput = new LogInput("D:\\" + Constant.Log, LogFormatEnum.TextFile);
            var log = new LogTextFile(logInput);
            ILogData logData = null;

            Assert.ThrowsException<LogException>(
                () => log.Write(logData),
                new LogException(nameof(logData), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void LogTextFileOtherExceptionsFailTest()
        {
            var logData = new LogData(new PingResponse());

            var logMock = new Mock<ILog>();

            logMock.Setup(v => v.Write(logData)).Throws<LogException>();

            Assert.ThrowsException<LogException>(
                () => logMock.Object.Write(logData));
        }

        #endregion

        #region ConfigJsonFile

        [TestMethod]
        public void ConfigJsonFileWriteSuccessTest()
        {
            var configData = new ConfigJsonData[] { new ConfigJsonData() };
            var configMock = new Mock<IConfig>();

            configMock.Setup(v => v.Write(configData));

            configMock.Object.Write(configData);
        }

        [TestMethod]
        public void ConfigJsonFileReadSuccessTest()
        {
            var configData = new ConfigJsonData[] { new ConfigJsonData() };
            var configMock = new Mock<IConfig>();

            configMock.Setup(v => v.Read()).Returns(configData);

            var result = configMock.Object.Read();

            Assert.IsInstanceOfType(result, configData.GetType());
        }

        [TestMethod]
        public void ConfigJsonFileWriteExceptionsFailTest()
        {
            var configData = new ConfigJsonData[] { new ConfigJsonData() };
            var configMock = new Mock<IConfig>();

            configMock.Setup(v => v.Write(configData)).Throws<ConfigException>();

            Assert.ThrowsException<ConfigException>(
                () => configMock.Object.Write(configData));
        }

        [TestMethod]
        public void ConfigJsonFileReadExceptionsFailTest()
        {
            var configMock = new Mock<IConfig>();

            configMock.Setup(v => v.Read()).Throws<ConfigException>();

            Assert.ThrowsException<ConfigException>(
                () => configMock.Object.Read());
        }

        [TestMethod]
        public void ConfigJsonFileCreateDefaultConfigSuccessTest()
        {
            var configData = new ConfigJsonData();
            var configMock = new Mock<IConfig>();

            configMock.Setup(v => v.CreateDefaultConfig(configData)).Returns(true);

            Assert.IsTrue(configMock.Object.CreateDefaultConfig(configData));
        }

        [TestMethod]
        public void ConfigJsonFileCreateDefaultConfigFailTest()
        {
            var configData = new ConfigJsonData();
            var configMock = new Mock<IConfig>();

            configMock.Setup(v => v.CreateDefaultConfig(configData)).Returns(false);

            Assert.IsFalse(configMock.Object.CreateDefaultConfig(configData));
        }

        #endregion

        #region PingRequestFactory

        public class OtherRequest : IPing<IHostInput, IPingResponse>
        {
            public IHostInput HostInput { get; }

            public IPingResponse Ping()
            {
                return new PingResponse();
            }

            public void Dispose()
            {

            }
        }

        public class OtherPingRequestFactory : IFactory<IConfigData, IPing<IHostInput, IPingResponse>>
        {
            public enum ProtocolTypeEnum
            {
                Http,
                Icmp,
                Tcp,
                Unknown = 1000
            }

            private ProtocolTypeEnum _protocolType;

            public OtherPingRequestFactory(ProtocolTypeEnum protocolType)
            {
                _protocolType = protocolType;
            }

            public IPing<IHostInput, IPingResponse> GetInstance(IConfigData configData)
            {
                switch (_protocolType)
                {
                    case ProtocolTypeEnum.Http:
                    case ProtocolTypeEnum.Icmp:
                    case ProtocolTypeEnum.Tcp:
                        return Mock.Of<IPing<IHostInput, IPingResponse>>(v => v.HostInput == new HostInput());
                    default:
                        {
                            throw new PingRequestException($"{nameof(configData.Protocol) } = { configData.Protocol } is unsupported", nameof(ArgumentException));
                        }
                }
            }
        }

        [TestMethod]
        public void PingFactoryGetHttpRequestInstanceSuccessTest()
        {
            var httpConfig = Mock.Of<IConfigData>(v => v.Host == "https://ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == ProtocolTypeEnum.Http && v.TimeOut == new TimeSpan(0, 0, 5));

            var factory = new PingRequestFactory();

            Assert.IsInstanceOfType(factory.GetInstance(httpConfig), typeof(HttpRequest));
        }

        [TestMethod]
        public void PingFactoryGetIcmpRequestInstanceSuccessTest()
        {
            var icmpConfig = Mock.Of<IConfigData>(v => v.Host == "ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == ProtocolTypeEnum.Icmp && v.TimeOut == new TimeSpan(0, 0, 5));

            var factory = new PingRequestFactory();

            using (var icmpRequest = factory.GetInstance(icmpConfig))
            {
                Assert.IsInstanceOfType(icmpRequest, typeof(IcmpRequest));
            }
        }

        [TestMethod]
        public void PingFactoryGetTcpRequestInstanceSuccessTest()
        {
            var tcpConfig = Mock.Of<IConfigData>(v => v.Host == "127.0.0.1:5000" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == ProtocolTypeEnum.Tcp && v.TimeOut == new TimeSpan(0, 0, 5));

            var factory = new PingRequestFactory();

            using (var tcpRequest = factory.GetInstance(tcpConfig))
            {
                Assert.IsInstanceOfType(tcpRequest, typeof(TcpRequest));
            }
        }

        [TestMethod]
        public void PingFactoryGetInstanceFailTest()
        {
            var icmpConfig = Mock.Of<IConfigData>(v => v.Host == "ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == ProtocolTypeEnum.Icmp && v.TimeOut == new TimeSpan(0, 0, 5));

            var factoryMock = new Mock<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();

            factoryMock.Setup(v => v.GetInstance(icmpConfig)).Returns(new OtherRequest());

            Assert.ThrowsException<AssertFailedException>(
                () => Assert.IsInstanceOfType(factoryMock.Object.GetInstance(icmpConfig), typeof(IcmpRequest)));
        }

        [TestMethod]
        public void PingFactoryConfigDataNullTest()
        {
            var factory = new PingRequestFactory();
            IConfigData configData = null;

            Assert.ThrowsException<PingRequestException>(
                () => factory.GetInstance(configData),
                new PingRequestException(nameof(configData), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void PingFactoryProtocolTypeUnsupportedTest()
        {
            var icmpConfig = Mock.Of<IConfigData>(v => v.Host == "ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == ProtocolTypeEnum.Icmp && v.TimeOut == new TimeSpan(0, 0, 5));

            var factory = new OtherPingRequestFactory(OtherPingRequestFactory.ProtocolTypeEnum.Unknown);

            Assert.ThrowsException<PingRequestException>(() => factory.GetInstance(icmpConfig),
                new PingRequestException($"Protocol = { OtherPingRequestFactory.ProtocolTypeEnum.Unknown } is unsupported", nameof(ArgumentException)).ToString());
        }

        #endregion

        #region LogFactory

        public class LogOther : ILog
        {
            public ILogInput LogInput { get; }


            public void Write(ILogData logData)
            {

            }

            public async Task WriteAsync(ILogData logData)
            {
                await Task.Delay(1);
            }

            public void Dispose()
            {

            }
        }

        public class LogOtherFactory : IFactory<ILogInput, ILog>
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

            public ILog GetInstance(ILogInput logInput)
            {
                switch (_logFormat)
                {
                    case LogFormatEnum.TextFile:
                        return new LogTextFile(logInput);
                    default:
                        throw new LogException($"{nameof(logInput.Format)} = {_logFormat} is unsupported", nameof(ArgumentException));
                }
            }
        }

        [TestMethod]
        public void LogFactoryGetLogTextFileInstanceSuccessTest()
        {
            var factory = new LogFactory();

            Assert.IsInstanceOfType(factory.GetInstance(new LogInput(Constant.Log, LogFormatEnum.TextFile)), typeof(LogTextFile));
        }

        [TestMethod]
        public void LogFactoryGetLogTextFileInstanceFailTest()
        {
            var factoryMock = new Mock<IFactory<Enum, ILog>>();
            factoryMock.Setup(v => v.GetInstance(LogFormatEnum.TextFile)).Returns(new LogOther());

            Assert.ThrowsException<AssertFailedException>(
                () => Assert.IsInstanceOfType(factoryMock.Object.GetInstance(LogFormatEnum.TextFile), typeof(LogTextFile)));
        }

        [TestMethod]
        public void LogFactoryLogTextFileInstanceFailTest()
        {
            var factoryMock = new Mock<IFactory<Enum, ILog>>();
            factoryMock.Setup(v => v.GetInstance(LogFormatEnum.TextFile)).Returns(new LogOther());

            Assert.ThrowsException<AssertFailedException>(
                () => Assert.IsInstanceOfType(factoryMock.Object.GetInstance(LogFormatEnum.TextFile), typeof(LogTextFile)));
        }

        [TestMethod]
        public void LogFactoryLogFormatNullTest()
        {
            var factory = new LogFactory();
            ILogInput logInput = null;

            Assert.ThrowsException<LogException>(
                () => factory.GetInstance(logInput),
                new LogException(nameof(logInput), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void LogFactoryLogFormatUnsupportedTest()
        {
            var factory = new LogOtherFactory(LogOtherFactory.LogFormatEnum.Unknown);

            Assert.ThrowsException<LogException>(
                () => factory.GetInstance(null),
                new LogException($"Format = {LogOtherFactory.LogFormatEnum.Unknown} is unsupported", nameof(ArgumentException)).ToString());
        }

        #endregion

        #region DefaultConfigFactory

        public class DefaultConfigOther : IConfigData
        {
            public string Host { get; set; }
            public TimeSpan Period { get; set; }
            public TimeSpan TimeOut { get; set; }
            public ProtocolTypeEnum Protocol { get; set; }
        }

        public class DefaultConfigOtherFactory : IFactory<IConfigInput, IConfigData>
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

            public IConfigData GetInstance(IConfigInput configFormat)
            {
                switch (_configFormat)
                {
                    case ConfigFormatEnum.JsonFile:
                        return new ConfigJsonData();
                    default:
                        throw new ConfigException($"{nameof(configFormat.Format)} = {_configFormat} is unsupported", nameof(ArgumentException));
                }
            }
        }

        [TestMethod]
        public void DefaultConfigFactoryGetConfigJosnDataInstanceSuccessTest()
        {
            var factory = new DefaultConfigFactory();

            Assert.IsInstanceOfType(factory.GetInstance(new ConfigInput("D:\\" + Constant.Config, ConfigFormatEnum.JsonFile)), typeof(ConfigJsonData));
        }

        [TestMethod]
        public void DefaultConfigFactoryGetConfigJsonDataInstanceFailTest()
        {
            var factoryMock = new Mock<IFactory<Enum, IConfigData>>();
            factoryMock.Setup(v => v.GetInstance(ConfigFormatEnum.JsonFile)).Returns(new DefaultConfigOther());

            Assert.ThrowsException<AssertFailedException>(
                () => Assert.IsInstanceOfType(factoryMock.Object.GetInstance(ConfigFormatEnum.JsonFile), typeof(ConfigJsonData)));
        }

        [TestMethod]
        public void DefaultConfigFactoryConfigFormatNullTest()
        {
            var factory = new DefaultConfigFactory();
            IConfigInput configInput = null;

            Assert.ThrowsException<ConfigException>(
                () => factory.GetInstance(configInput),
                new ConfigException(nameof(configInput), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void DefaultConfigFactoryConfigFormatUnsupportedTest()
        {
            var factory = new DefaultConfigOtherFactory(DefaultConfigOtherFactory.ConfigFormatEnum.Unknown);

            Assert.ThrowsException<ConfigException>(
                () => factory.GetInstance(null),
                new ConfigException($"Format = {DefaultConfigOtherFactory.ConfigFormatEnum.Unknown} is unsupported", nameof(ArgumentException)).ToString());
        }

        #endregion

        #region ConfigFactory

        public class ConfigOtherFactory : IFactory<IConfigInput, IConfig>
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

            public IConfig GetInstance(IConfigInput configInput)
            {
                switch (_configFormat)
                {
                    case ConfigFormatEnum.JsonFile:
                        return new ConfigJsonFile<ConfigJsonData>(configInput);
                    default:
                        throw new ConfigException($"{nameof(configInput.Format)} = {_configFormat} is unsupported", nameof(ArgumentException));
                }
            }
        }

        public class ConfigOther : IConfig
        {
            public IConfigInput ConfigInput { get; }

            public bool CreateDefaultConfig(IConfigData configData)
            {
                return true;
            }

            public IEnumerable<IConfigData> Read()
            {
                return null;
            }

            public void Write(IEnumerable<IConfigData> configData)
            {

            }
        }

        [TestMethod]
        public void ConfigFactoryGetConfigJosnFileInstanceSuccessTest()
        {
            var factory = new ConfigFactory();

            Assert.IsInstanceOfType(factory.GetInstance(new ConfigInput("D:\\" + Constant.Config, ConfigFormatEnum.JsonFile)), typeof(ConfigJsonFile<ConfigJsonData>));
        }

        [TestMethod]
        public void ConfigFactoryGetConfigJsonFileInstanceFailTest()
        {
            var factoryMock = new Mock<IFactory<Enum, IConfig>>();
            factoryMock.Setup(v => v.GetInstance(ConfigFormatEnum.JsonFile)).Returns(new ConfigOther());

            Assert.ThrowsException<AssertFailedException>(
                () => Assert.IsInstanceOfType(factoryMock.Object.GetInstance(ConfigFormatEnum.JsonFile), typeof(ConfigJsonFile<ConfigJsonData>)));
        }

        [TestMethod]
        public void ConfigFactoryConfigFormatNullTest()
        {
            var factory = new ConfigFactory();
            IConfigInput configInput = null;

            Assert.ThrowsException<ConfigException>(
                () => factory.GetInstance(configInput),
                new ConfigException(nameof(configInput), nameof(ArgumentNullException)).ToString());
        }

        [TestMethod]
        public void ConfigFactoryConfigFormatUnsupportedTest()
        {
            var factory = new ConfigOtherFactory(ConfigOtherFactory.ConfigFormatEnum.Unknown);

            Assert.ThrowsException<ConfigException>(
                () => factory.GetInstance(null),
                new ConfigException($"Format = {ConfigOtherFactory.ConfigFormatEnum.Unknown} is unsupported", nameof(ArgumentException)).ToString());
        }

        #endregion

        #region PingManager

        [TestMethod]
        public void PingManagerCreateInstanceSuccessTest()
        {
            var pingRequestFactory = Mock.Of<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();
            var config = Mock.Of<IConfig>();
            var log = Mock.Of<ILog>();

            var pingManager = new PingManager(pingRequestFactory, config, log);

            Assert.IsInstanceOfType(pingManager, typeof(PingManager));
        }

        [TestMethod]
        public void PingManagerNullArgumentsFailTest()
        {
            var pingRequestFactory = Mock.Of<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();
            var config = Mock.Of<IConfig>();
            var log = Mock.Of<ILog>();

            Assert.ThrowsException<ArgumentNullException>(
                () => new PingManager(null, config, log),
                new ArgumentNullException(nameof(pingRequestFactory)).Message);
            Assert.ThrowsException<ArgumentNullException>(
                () => new PingManager(pingRequestFactory, null, log),
                new ArgumentNullException(nameof(config)).Message);
            Assert.ThrowsException<ArgumentNullException>(
                () => new PingManager(pingRequestFactory, config, null),
                new ArgumentNullException(nameof(log)).Message);
        }

        [TestMethod]
        public void PingManagerStartReadConfigSuccessTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.ConfigSuccessRead, Constant.Config);
            string response = null;

            pingManagerMock.Setup(v => v.Start()).Callback(() => response = expected);
            pingManagerMock.Object.Start();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerStartPingRequestSuccessTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = Constant.PingRequestStart;
            string response = null;

            pingManagerMock.Setup(v => v.Start()).Callback(() => response = expected);
            pingManagerMock.Object.Start();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerStartReadConfigFailTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.ConfigFailRead, Constant.Config, new ConfigException());
            string response = null;

            pingManagerMock.Setup(v => v.Start()).Callback(() => response = expected);
            pingManagerMock.Object.Start();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerStartPingRequestInstanceFailTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.PingRequestInstanceFail, ProtocolTypeEnum.Http, new PingRequestException());
            string response = null;

            pingManagerMock.Setup(v => v.Start()).Callback(() => response = expected);
            pingManagerMock.Object.Start();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerStartPingRequestFailTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.PingRequestFail, ProtocolTypeEnum.Http, new PingRequestException());
            string response = null;

            pingManagerMock.Setup(v => v.Start()).Callback(() => response = expected);
            pingManagerMock.Object.Start();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerStartLogWriteFailTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.LogFailWrite, Constant.Log, new LogException());
            string response = null;

            pingManagerMock.Setup(v => v.Start()).Callback(() => response = expected);
            pingManagerMock.Object.Start();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerStopTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = Constant.PingRequestStop;
            string response = null;

            pingManagerMock.Setup(v => v.Stop()).Callback(() => response = expected);
            pingManagerMock.Object.Stop();

            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerCheckConfigCreateFailTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.ConfigFailCreate, Constant.Config, new ConfigException());
            string response = null;

            var configData = Mock.Of<IConfigData>();
            pingManagerMock.Setup(v => v.CheckConfig(configData)).Returns(() => false).Callback(() => response = expected);

            Assert.IsFalse(pingManagerMock.Object.CheckConfig(configData));
            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerCheckConfigNotFoundButCreatedTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            string expected = string.Format(Constant.ConfigNotFoundButCreated, Constant.Config);
            string response = null;

            var configData = Mock.Of<IConfigData>();
            pingManagerMock.Setup(v => v.CheckConfig(configData)).Returns(() => false).Callback(() => response = expected);

            Assert.IsFalse(pingManagerMock.Object.CheckConfig(configData));
            Assert.AreEqual(expected, response);
        }

        [TestMethod]
        public void PingManagerCheckConfigCreateSuccessTest()
        {
            var pingManagerMock = new Mock<IPingManager>();
            var configData = Mock.Of<IConfigData>();

            pingManagerMock.Setup(v => v.CheckConfig(configData)).Returns(() => true);
            Assert.IsTrue(pingManagerMock.Object.CheckConfig(configData));
        }

        [TestMethod]
        public void PingManagerCheckConfigNullArgumentFailTest()
        {
            var pingRequestFactory = Mock.Of<IFactory<IConfigData, IPing<IHostInput, IPingResponse>>>();
            var config = Mock.Of<IConfig>();
            var log = Mock.Of<ILog>();
            IConfigData configData = null;

            var pingManager = new PingManager(pingRequestFactory, config, log);

            Assert.ThrowsException<ArgumentNullException>(
                () => pingManager.CheckConfig(configData),
                new ArgumentNullException(nameof(configData)).Message);
        }

        #endregion
    }
}