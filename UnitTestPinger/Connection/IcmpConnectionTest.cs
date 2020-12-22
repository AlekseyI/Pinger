using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger.Connection;
using Pinger.Enums;
using Pinger.Input;
using System;

namespace UnitTestPinger.Connection
{
    [TestClass]
    public class IcmpConnectionTest
    {
        [TestMethod]
        public void IcmpPingSuccessTest()
        {
            using (var icmpRequest = new IcmpConnection(new HostInput("ya.ru", new TimeSpan(0, 0, 5))))
            {
                icmpRequest.Ping().Wait();
                Assert.IsTrue(icmpRequest.Response.Status == PingStatus.Ok);
            }
        }

        [TestMethod]
        public void IcmpPingInvalidHostFormatFailTest()
        {
            Assert.ThrowsException<FormatException>(() => new IcmpConnection(new HostInput("ya", new TimeSpan(0, 0, 5))));
        }

        [TestMethod]
        public void IcmpPingTimeOutFailTest()
        {
            using (var icmpRequest = new IcmpConnection(new HostInput("10.255.255.1", new TimeSpan(0, 0, 1))))
            {
                icmpRequest.Ping().Wait();
                Assert.IsTrue(icmpRequest.Response.Status == PingStatus.TimeOut);
            }
        }

        [TestMethod]
        public void IcmpPingHostInputNullFailTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new IcmpConnection(null));
        }

        [TestMethod]
        public void IcmpPingHostInputAddressNullOrEmptyFailTest()
        {
            Assert.ThrowsException<ArgumentException>(() => new IcmpConnection(new HostInput(null, new TimeSpan(0, 0, 5))));
            Assert.ThrowsException<ArgumentException>(() => new IcmpConnection(new HostInput("", new TimeSpan(0, 0, 5))));
        }

        [TestMethod]
        public void IcmpPingTimeOutOverFlowFailTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IcmpConnection(new HostInput("ya.ru", TimeSpan.Zero)));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IcmpConnection(new HostInput("ya.ru", new TimeSpan(0, 0, -1))));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IcmpConnection(new HostInput("ya.ru", new TimeSpan(25, 0, 0, 0, 0))));
        }
    }
}
