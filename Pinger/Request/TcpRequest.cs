using Pinger.Enums;
using Pinger.Exceptions;
using Pinger.Input;
using Pinger.Response;
using System;
using System.Net.Sockets;

namespace Pinger.Request
{
    public class TcpRequest : IPing<IHostPortInput, IPingCodeResponse>
    {
        private Socket _socket;

        public IHostPortInput HostInput { get; }

        public TcpRequest(IHostPortInput hostInput)
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
            else if (hostInput.Port <= 0 || hostInput.Port > int.MaxValue)
            {
                throw new PingRequestException(nameof(hostInput.Port), nameof(ArgumentOutOfRangeException));
            }
            else
            {
                IHostInputCheck inputCheck = new HostInputValidate();

                if (!inputCheck.Check(hostInput.Address, Constant.TcpIpCheckPattern))
                {
                    throw new PingRequestException(nameof(hostInput.Address), nameof(FormatException));
                }
                
                HostInput = hostInput;
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
        }

        public IPingCodeResponse Ping()
        {
            PingStatusEnum status;
            int code = -1;
            IAsyncResult result = null;
            try
            {
                var hostPort = HostInput;
                result = _socket.BeginConnect(hostPort.Address, hostPort.Port, null, null);
                var isTimeOut = !result.AsyncWaitHandle.WaitOne(hostPort.TimeOut, true);
                
                if (_socket.Connected)
                {
                    status = PingStatusEnum.Ok;
                    code = (int)SocketError.Success;
                }
                else if (isTimeOut)
                {
                    status = PingStatusEnum.TimeOut;
                }
                else
                {
                    status = PingStatusEnum.Fail;
                }
            }
            catch(SocketException e)
            {
                code = (int)e.SocketErrorCode;
                status = PingStatusEnum.Fail;
            }
            catch (Exception e)
            {
                throw new PingRequestException(e.Message, e.GetType().Name);
            }
            finally
            {
                if (result != null)
                {
                    _socket.EndConnect(result);
                }
            }

            return new PingCodeResponse(DateTime.Now, ProtocolTypeEnum.Tcp, HostInput.Address, status, code);
        }

        public void Dispose()
        {
            _socket?.Close();
            _socket = null;
        }
    }
}