using Pinger.Enums;
using Pinger.Input;
using Pinger.Response;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Pinger.Connection
{
    public class IcmpConnection : IPing<IHostInput, IPingResponse>
    {
        private Ping _ping;

        public IHostInput HostInput { get; }
        public IPingResponse Response { get; private set; }

        public IcmpConnection(IHostInput hostInput)
        {
            if (hostInput == null)
            {
                throw new ArgumentNullException(nameof(hostInput));
            }
            else if (string.IsNullOrEmpty(hostInput.Address))
            {
                throw new ArgumentException(nameof(hostInput.Address));
            }
            else if (hostInput.TimeOut.TotalMilliseconds <= 0 || hostInput.TimeOut.TotalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(hostInput.TimeOut));
            }
            else
            {
                IHostInputCheck inputCheck = new HostInputValidate();
               
                if (!inputCheck.Check(hostInput.Address, Constant.IcmpUrlOrIpCheckPattern))
                {
                    throw new FormatException(nameof(hostInput.Address));
                }

                HostInput = hostInput;
                _ping = new Ping();
            }
        }

        public async Task Ping()
        {
            PingStatus status;
            try
            {
                var response = await _ping.SendPingAsync(HostInput.Address, (int)HostInput.TimeOut.TotalMilliseconds);
                
                if (response.Status == IPStatus.Success)
                {
                    status = PingStatus.Ok;
                }
                else if (response.Status == IPStatus.TimedOut)
                {
                    status = PingStatus.TimeOut;
                }
                else
                {
                    status = PingStatus.Fail;
                }
            }
            catch (PingException)
            {
                status = PingStatus.Fail;
            }

            Response = new PingResponse(DateTime.Now, TypeProtocol.Icmp, HostInput.Address, status);
        }

        public void Dispose()
        {
            _ping?.Dispose();
            _ping = null;
        }
    }
}