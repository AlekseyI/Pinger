using Pinger.Config;
using Pinger.Enums;
using Pinger.Input;
using Pinger.Connection;
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
                throw new ArgumentNullException(nameof(configData));
            }

            switch (configData.Protocol)
            {
                case TypeProtocol.Http:
                    {
                        return new HttpConnection(new HostInput(configData.Host, configData.TimeOut));
                    }
                case TypeProtocol.Icmp:
                    {
                        return new IcmpConnection(new HostInput(configData.Host, configData.TimeOut));
                    }
                case TypeProtocol.Tcp:
                    {
                        IHostInputParse inputParser = new HostInputValidate();
                        var (ip, port) = inputParser.Parse(configData.Host);
                        return new TcpConnection(new HostPortInput(ip, port, configData.TimeOut));
                    }
                default:
                    {
                        throw new ArgumentException($"{nameof(configData.Protocol) } = { configData.Protocol } is unsupported");
                    }
            }
        }
    }
}