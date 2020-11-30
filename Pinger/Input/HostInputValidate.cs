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
                throw new ArgumentNullException(nameof(host));
            }
            else if (formatPattern == null)
            {
                throw new ArgumentNullException(nameof(formatPattern));
            }
            else
            {
                return Regex.IsMatch(host, formatPattern, RegexOptions.Compiled);
            }
        }

        public (string, int) Parse(string host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            var ipPortMatch = Regex.Match(host, Constant.TcpIpPortPattern, RegexOptions.Compiled);

            if (ipPortMatch.Success)
            { 
                return (ipPortMatch.Groups[1].Value, int.Parse(ipPortMatch.Groups[2].Value));
            }
            else
            {
                throw new FormatException(nameof(host));
            }
        }
    }
}