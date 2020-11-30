using System;
using Pinger.Response;
using Pinger.Input;
using Pinger.Enums;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace Pinger.Connection
{
    public class HttpConnection : IPing<IHostInput, IPingCodeResponse>
    {
        public IHostInput HostInput { get; }
        public IPingCodeResponse Response { get; private set; }

        public HttpConnection(IHostInput hostInput)
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

                if (!inputCheck.Check(hostInput.Address, Constant.HttpUrlOrIpCheckPattern))
                {
                    throw new FormatException(nameof(hostInput.Address));
                }

                HostInput = hostInput;
            }
        }

        public async Task Ping()
        {
            PingStatus status;
            int code = -1;
            var httpClient = new HttpClient();

            try
            {
                httpClient.Timeout = HostInput.TimeOut;
                var result = await httpClient.GetAsync(HostInput.Address);
                status = result.IsSuccessStatusCode ? PingStatus.Ok : PingStatus.Fail;
                code = result != null ? (int)result.StatusCode : code;
            }
            catch (HttpRequestException)
            {  
                status = PingStatus.Fail;
            }
            catch(TaskCanceledException)
            {
                status = PingStatus.TimeOut;
            }
            finally
            {
                httpClient.Dispose();
            }

            Response = new PingCodeResponse(DateTime.Now, TypeProtocol.Http, HostInput.Address, status, code);
        }

        public void Dispose() { }
    }
}