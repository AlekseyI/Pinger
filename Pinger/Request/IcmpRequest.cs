using Pinger.Enums;
using Pinger.Exceptions;
using Pinger.Input;
using Pinger.Response;
using System;
using System.Net.NetworkInformation;

namespace Pinger.Request
{
    public class IcmpRequest : IPing<IHostInput, IPingResponse>
    {
        private Ping _ping;

        public IHostInput HostInput { get; }

        public IcmpRequest(IHostInput hostInput)
        {
            if (hostInput == null)
            {
                throw new PingRequestException(nameof(hostInput), nameof(ArgumentNullException));
            }
            else if (string.IsNullOrEmpty(hostInput.Address))
            {
                throw new PingRequestException(nameof(hostInput.Address), nameof(ArgumentException));
            }
            else if (hostInput.TimeOut.TotalMilliseconds <= 0 || hostInput.TimeOut.TotalMilliseconds > int.MaxValue)
            {
                throw new PingRequestException(nameof(hostInput.TimeOut), nameof(ArgumentOutOfRangeException));
            }
            else
            {
                IHostInputCheck inputCheck = new HostInputValidate();
               
                if (!inputCheck.Check(hostInput.Address, Constant.IcmpUrlOrIpCheckPattern))
                {
                    throw new PingRequestException(nameof(hostInput.Address), nameof(FormatException));
                }

                HostInput = hostInput;
                _ping = new Ping();
            }
        }

        public IPingResponse Ping()
        {
            PingStatusEnum status;
            try
            {
                var response = _ping.Send(HostInput.Address, (int)HostInput.TimeOut.TotalMilliseconds);
                
                if (response.Status == IPStatus.Success)
                {
                    status = PingStatusEnum.Ok;
                }
                else if (response.Status == IPStatus.TimedOut)
                {
                    status = PingStatusEnum.TimeOut;
                }
                else
                {
                    status = PingStatusEnum.Fail;
                }
            }
            catch (PingException)
            {
                status = PingStatusEnum.Fail;
            }
            catch (Exception e)
            {
                throw new PingRequestException(e.Message, e.GetType().Name);
            }

            return new PingResponse(DateTime.Now, ProtocolTypeEnum.Icmp, HostInput.Address, status);
        }

        public void Dispose()
        {
            _ping?.Dispose();
            _ping = null;
        }
    }
}