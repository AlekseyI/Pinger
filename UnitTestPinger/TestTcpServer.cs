using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UnitTestPinger
{
    public class TestTcpServer : IDisposable
    {
        public const string ServerWaitStatus = "Server wait connect";
        public const string ServerClosedStatus = "Server closed";

        private Socket _socket;
        private int _maxQueue;
        private Thread _thread;

        public delegate void Status(string value);
        public event Status EventStatus;

        public TestTcpServer(int maxQueue)
        {
            if (maxQueue < 0)
            {
                throw new ArgumentException($"{nameof(maxQueue)} is not be negative");
            }

            _maxQueue = maxQueue;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(int port)
        {
            if (port < 0)
            {
                throw new ArgumentException($"{nameof(port)} is not be negative");
            }
            else if (_thread != null)
            {
                return;
            }
            else
            {
                _thread = new Thread(() =>
                {
                    _socket.Bind(new IPEndPoint(IPAddress.Any, port));

                    _socket.Listen(_maxQueue);

                    try
                    {
                        while (true)
                        {
                            EventStatus?.Invoke(ServerWaitStatus);

                            var client = _socket.Accept();

                            client.Close();
                        }
                    }
                    catch (SocketException)
                    {
                        EventStatus?.Invoke(ServerClosedStatus);
                    }
                });

                _thread.Start();
            }
        }

        public void Stop()
        {
            _socket?.Close();
            _thread?.Abort();
            _thread = null;
        }

        public void Dispose()
        {
            Stop();
            _socket = null;

            var lstDlg = EventStatus?.GetInvocationList();
            if(lstDlg != null)
            {
                foreach (var dlg in lstDlg)
                {
                    EventStatus -= (Status)dlg;
                }
            }
        }
    }
}