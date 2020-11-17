using Pinger.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace Pinger.Input
{
    public class HostInputValidate : IHostInputParse, IHostInputCheck
    {
        public bool Check(string host, string formatPattern)
        {
            if (host == null)
            {
                throw new PingRequestException(nameof(host), nameof(ArgumentNullException));
            }
            else if (formatPattern == null)
            {
                throw new PingRequestException(nameof(formatPattern), nameof(ArgumentNullException));
            }
            else
            {
                try
                {
                    return Regex.IsMatch(host, formatPattern, RegexOptions.Compiled);
                }
                catch(RegexMatchTimeoutException e)
                {
                    throw new PingRequestException(e.Message, nameof(RegexMatchTimeoutException));
                }
                catch(ArgumentOutOfRangeException e)
                {
                    throw new PingRequestException(e.Message, nameof(ArgumentOutOfRangeException));
                }
            }
        }

        public (string, int) Parse(string host)
        {
            if (host == null)
            {
                throw new PingRequestException(nameof(host), nameof(ArgumentNullException));
            }

            Match ipPortMatch;
            try
            {
                ipPortMatch = Regex.Match(host, Constant.TcpIpPortPattern, RegexOptions.Compiled);
            }
            catch (RegexMatchTimeoutException e)
            {
                throw new PingRequestException(e.Message, nameof(RegexMatchTimeoutException));
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new PingRequestException(e.Message, nameof(ArgumentOutOfRangeException));
            }

            if (ipPortMatch.Success)
            {
                try
                {
                    return (ipPortMatch.Groups[1].Value, int.Parse(ipPortMatch.Groups[2].Value));
                }
                catch(OverflowException)
                {
                    throw new PingRequestException(nameof(host), nameof(OverflowException));
                } 
            }
            else
            {
                throw new PingRequestException(nameof(host), nameof(FormatException));
            }
        }
    }
}