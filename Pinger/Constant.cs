namespace Pinger
{
    public class Constant
    {
        public const string Config = "config.conf";
        public const string Log = "log.log";

        public const string TcpIpPortPattern = @"^([1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9])[:](\d+)$";

        public const string TcpIpCheckPattern = @"^[1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9]$";

        public const string HttpUrlOrIpCheckPattern = @"^http[s]?[:]//(?:(?:\w+(?:[-.]?\w+)*[.][a-zA-Z]+)|(?:[1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9]))(?:[:]\d+)?(?:[/].*)?$";

        public const string IcmpUrlOrIpCheckPattern = @"(?:^\w+(?:[-.]?\w+)*[.][a-zA-Z]+$)|(?:^[1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9]$)";
    }
}
