using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinger;
using Pinger.Input;
using System;

namespace UnitTestPinger
{
    [TestClass]
    public class HostInputValidateTest
    {
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

            Assert.ThrowsException<ArgumentNullException>(() => hostInputCheck.Check(null, Constant.HttpUrlOrIpCheckPattern));
            Assert.ThrowsException<ArgumentNullException>(() => hostInputCheck.Check("https://ya.ru", null));
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

            Assert.ThrowsException<FormatException>(() => hostInputCheck.Parse("173.194.216.113"));
            Assert.ThrowsException<FormatException>(() => hostInputCheck.Parse("173.194.216:80"));
            Assert.ThrowsException<FormatException>(() => hostInputCheck.Parse("173.194.216.113:"));
        }

        [TestMethod]
        public void HostInputParseHostNullFailTest()
        {
            var hostInputCheck = new HostInputValidate();

            Assert.ThrowsException<ArgumentNullException>(() => hostInputCheck.Parse(null));
        }
    }
}
