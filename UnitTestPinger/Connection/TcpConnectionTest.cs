using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pinger.Connection;
using Pinger.Enums;
using Pinger.Input;
using Pinger.Response;
using System;
using System.Threading;

namespace UnitTestPinger.Connection
{
    [TestClass]
    public class TcpConnectionTest
    {
        private string _serverOutput;

        private static int _port = 11000;
        private string _host = "127.0.0.1:" + _port;

        private void EventHandler(string status)
        {
            _serverOutput = status;
        }

        [TestMethod]
        public void TcpPingSuccessTest()
        {
            using (var server = new TestTcpServer(5))
            {
                server.EventStatus += EventHandler;
                server.Start(_port);

                Thread.Sleep(1000);
                Assert.AreEqual(TestTcpServer.ServerWaitStatus, _serverOutput);

                using (var tcpRequest = new TcpConnection(new HostInput(_host, new TimeSpan(0, 0, 5))))
                {
                    tcpRequest.Ping().Wait();
                    Assert.IsTrue(tcpRequest.Response.Status == PingStatus.Ok && tcpRequest.Response.Code == 0);
                }
            }
            _serverOutput = null;
        }

        [TestMethod]
        public void TcpPingInvalidHostFormatFailTest()
        {
            Assert.ThrowsException<FormatException>(() => new TcpConnection(new HostInput("127.0.0:", new TimeSpan(0, 0, 5))));
        }

        [TestMethod]
        public void TcpPingTimeOutFailTest()
        {
            var tcpRequestMock = new Mock<IPing<IHostInput, IPingCodeResponse>>();
            var expected = new PingCodeResponse(DateTime.Now, TypeProtocol.Tcp, _host, PingStatus.TimeOut, -1);

            tcpRequestMock.Setup(v => v.Response).Returns(expected);
            Assert.AreEqual(expected.ToString(), tcpRequestMock.Object.Response.ToString());
        }

        [TestMethod]
        public void TcpPingHostInputNullFailTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TcpConnection(null));
        }

        [TestMethod]
        public void TcpPingHostInputAddressNullOrEmptyFailTest()
        {
            Assert.ThrowsException<ArgumentException>(() => new TcpConnection(new HostInput(null, new TimeSpan(0, 0, 5))));
            Assert.ThrowsException<ArgumentException>(() => new TcpConnection(new HostInput("", new TimeSpan(0, 0, 5))));
        }

        [TestMethod]
        public void TcpPingTimeOutOverFlowFailTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TcpConnection(new HostInput(_host, TimeSpan.Zero)));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TcpConnection(new HostInput(_host, new TimeSpan(0, 0, -1))));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TcpConnection(new HostInput(_host, new TimeSpan(25, 0, 0, 0, 0))));
        }
    }
}
