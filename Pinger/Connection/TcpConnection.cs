using Pinger.Enums;
using Pinger.Input;
using Pinger.Response;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pinger.Connection
{
    public class TcpConnection : IPing<IHostInput, IPingCodeResponse>
    {
        private Socket _socket;

        public IHostInput HostInput { get; }
        public IPingCodeResponse Response { get; private set; }

        private string _ip;
        private int _port;

        public TcpConnection(IHostInput hostInput)
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
                var inputParser = new HostInputValidate();
                (_ip, _port) = inputParser.Parse(hostInput.Address);

                if (_port <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(_port));
                }

                HostInput = hostInput;
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
        }

        public async Task Ping()
        {
            PingStatus status;
            int code = -1;

            try
            {
                var request = _socket.ConnectAsync(_ip, _port);
                await await Task.WhenAny(request, Task.Delay(HostInput.TimeOut));

                if (!request.IsCompleted)
                {
                    status = PingStatus.TimeOut;
                }
                else if (_socket.Connected)
                {
                    status = PingStatus.Ok;
                    code = (int)SocketError.Success;
                }
                else
                {
                    status = PingStatus.Fail;
                }
            }
            catch (SocketException e)
            {
                code = (int)e.SocketErrorCode;
                status = PingStatus.Fail;
            }

            Response = new PingCodeResponse(DateTime.Now, TypeProtocol.Tcp, HostInput.Address, status, code);
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;
        }
    }
}