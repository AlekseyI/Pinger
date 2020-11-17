using System;
using Pinger.Response;
using Pinger.Input;
using Pinger.Enums;
using System.Net;
using Pinger.Exceptions;

namespace Pinger.Request
{
    public class HttpRequest : IPing<IHostInput, IPingCodeResponse>
    {
        public IHostInput HostInput { get; }

        public HttpRequest(IHostInput hostInput)
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

                if (!inputCheck.Check(hostInput.Address, Constant.HttpUrlOrIpCheckPattern))
                {
                    throw new PingRequestException(nameof(hostInput.Address), nameof(FormatException));
                }

                HostInput = hostInput;
            }
        }

        public IPingCodeResponse Ping()
        {
            PingStatusEnum status;
            int code = -1;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HostInput.Address);
                request.Timeout = (int)HostInput.TimeOut.TotalMilliseconds;
                request.AllowAutoRedirect = false;
                request.Method = WebRequestMethods.Http.Head;
                
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    code = (int)response.StatusCode;
                    status = PingStatusEnum.Ok;
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    status = PingStatusEnum.TimeOut;
                }
                else
                {
                    status = PingStatusEnum.Fail;
                }
            }
            catch(UriFormatException)
            {
                status = PingStatusEnum.Fail;
            }
            catch (Exception e)
            {
                throw new PingRequestException(e.Message, e.GetType().Name);
            }

            return new PingCodeResponse(DateTime.Now, ProtocolTypeEnum.Http, HostInput.Address, status, code);
        }

        public void Dispose() { }
    }
}