using Pinger.Enums;
using Pinger.Input;
using Pinger.Response;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pinger.Connection
{
    public class TcpConnection : IPing<IHostPortInput, IPingCodeResponse>
    {
        private Socket _socket;

        public IHostPortInput HostInput { get; }
        public IPingCodeResponse Response { get; private set; }

        public TcpConnection(IHostPortInput hostInput)
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
            else if (hostInput.Port <= 0 || hostInput.Port > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(hostInput.Port));
            }
            else
            {
                IHostInputCheck inputCheck = new HostInputValidate();

                if (!inputCheck.Check(hostInput.Address, Constant.TcpIpCheckPattern))
                {
                    throw new FormatException(nameof(hostInput.Address));
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
                var request = _socket.ConnectAsync(HostInput.Address, HostInput.Port);
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