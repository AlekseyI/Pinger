using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger.Connection;
using Pinger.Enums;
using Pinger.Input;
using System;

namespace UnitTestPinger.Connection
{
    [TestClass]
    public class HttpConnectionTest
    {
        [TestMethod]
        public void HttpPingSuccessTest()
        {
            using (var httpRequest = new HttpConnection(new HostInput("https://google.com", new TimeSpan(0, 0, 5))))
            {
                httpRequest.Ping().Wait();
                Assert.IsTrue(httpRequest.Response.Status == PingStatus.Ok && httpRequest.Response.Code == 200);
            }
        }

        [TestMethod]
        public void HttpPingInvalidHostFormatFailTest()
        {
            Assert.ThrowsException<FormatException>(() => new HttpConnection(new HostInput("https:ya.ru", new TimeSpan(0, 0, 5))));
        }

        [TestMethod]
        public void HttpPingTimeOutFailTest()
        {
            using (var httpRequest = new HttpConnection(new HostInput("https://google.com:81", new TimeSpan(0, 0, 1))))
            {
                httpRequest.Ping().Wait();
                Assert.IsTrue(httpRequest.Response.Status == PingStatus.TimeOut);
            }
        }

        [TestMethod]
        public void HttpPingHostInputNullFailTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new HttpConnection(null));
        }

        [TestMethod]
        public void HttpPingHostInputAddressNullOrEmptyFailTest()
        {
            Assert.ThrowsException<ArgumentException>(() => new HttpConnection(new HostInput(null, new TimeSpan(0, 0, 5))));
            Assert.ThrowsException<ArgumentException>(() => new HttpConnection(new HostInput("", new TimeSpan(0, 0, 5))));
        }

        [TestMethod]
        public void HttpPingTimeOutOverFlowFailTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new HttpConnection(new HostInput("https://ya.ru", TimeSpan.Zero)));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new HttpConnection(new HostInput("https://ya.ru", new TimeSpan(0, 0, -1))));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new HttpConnection(new HostInput("https://ya.ru", new TimeSpan(25, 0, 0, 0, 0))));
        }
    }
}
