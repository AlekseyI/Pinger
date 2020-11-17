using Pinger.Config;
using Pinger.Enums;
using Pinger.Exceptions;
using Pinger.Input;
using Pinger.Request;
using Pinger.Response;
using System;

namespace Pinger.Factory.Ping
{
    public class PingRequestFactory : IFactory<IConfigData, IPing<IHostInput, IPingResponse>>
    {
        public IPing<IHostInput, IPingResponse> GetInstance(IConfigData configData)
        {
            if (configData == null)
            {
                throw new PingRequestException(nameof(configData), nameof(ArgumentNullException));
            }

            switch (configData.Protocol)
            {
                case ProtocolTypeEnum.Http:
                    {
                        return new HttpRequest(new HostInput(configData.Host, configData.TimeOut));
                    }
                case ProtocolTypeEnum.Icmp:
                    {
                        return new IcmpRequest(new HostInput(configData.Host, configData.TimeOut));
                    }
                case ProtocolTypeEnum.Tcp:
                    {
                        IHostInputParse inputParser = new HostInputValidate();
                        var (ip, port) = inputParser.Parse(configData.Host);
                        return new TcpRequest(new HostPortInput(ip, port, configData.TimeOut));
                    }
                default:
                    {
                        throw new PingRequestException($"{nameof(configData.Protocol) } = { configData.Protocol } is unsupported", nameof(ArgumentException));
                    }
            }
        }
    }
}