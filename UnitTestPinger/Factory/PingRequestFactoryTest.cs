using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pinger.Config;
using Pinger.Connection;
using Pinger.Factory;
using Pinger.Factory.Ping;
using Pinger.Input;
using Pinger.Response;
using System;
using Pinger.Enums;
using System.Threading.Tasks;

namespace UnitTestPinger.Factory
{
    [TestClass]
    public class PingRequestFactoryTest
    {
        [TestMethod]
        public void PingFactoryGetHttpRequestInstanceSuccessTest()
        {
            var config = Mock.Of<IConfigData>(v => v.Host == "https://ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == TypeProtocol.Http && v.TimeOut == new TimeSpan(0, 0, 5));

            Assert.IsInstanceOfType(new PingRequestFactory().GetInstance(config), typeof(HttpConnection));
        }

        [TestMethod]
        public void PingFactoryGetIcmpRequestInstanceSuccessTest()
        {
            var config = Mock.Of<IConfigData>(v => v.Host == "ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == TypeProtocol.Icmp && v.TimeOut == new TimeSpan(0, 0, 5));

            Assert.IsInstanceOfType(new PingRequestFactory().GetInstance(config), typeof(IcmpConnection));
        }

        [TestMethod]
        public void PingFactoryGetTcpRequestInstanceSuccessTest()
        {
            var config = Mock.Of<IConfigData>(v => v.Host == "127.0.0.1:5000" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == TypeProtocol.Tcp && v.TimeOut == new TimeSpan(0, 0, 5));

            Assert.IsInstanceOfType(new PingRequestFactory().GetInstance(config), typeof(TcpConnection));
        }

        [TestMethod]
        public void PingFactoryConfigDataNullTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PingRequestFactory().GetInstance(null));
        }

        [TestMethod]
        public void PingFactoryProtocolTypeUnsupportedTest()
        {
            var icmpConfig = Mock.Of<IConfigData>(v => v.Host == "ya.ru" && v.Period == new TimeSpan(0, 0, 1) && v.Protocol == TypeProtocol.Icmp && v.TimeOut == new TimeSpan(0, 0, 5));

            var factory = new OtherPingRequestFactory(OtherPingRequestFactory.ProtocolTypeEnum.Unknown);

            Assert.ThrowsException<ArgumentException>(() => factory.GetInstance(icmpConfig));
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
                            throw new ArgumentException($"{nameof(configData.Protocol) } = { configData.Protocol } is unsupported");
                        }
                }
            }
        }
}
}
