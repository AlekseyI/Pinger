namespace Pinger
{
    public class Constant
    {
        public const string Config = "config.conf";
        public const string Log = "log.log";

        public const string ConfigSuccessRead = "Данные из {0} успешно получены";
        public const string ConfigFailRead = "Ошибка при получения данных из {0} : {1}"; 
        public const string ConfigFailCreate = "Ошибка создания {0} : {1}"; 
        public const string ConfigNotFoundButCreated = "{0} не был найден, поэтому он был создан, заполните его";

        public const string LogFailWrite = "Ошибка записи данных в {0} : {1}";

        public const string PingRequestInstanceFail = "Ошибка при создании экземпляра протокола {0} : {1}";
        public const string PingRequestStart = "Ping запущен";
        public const string PingRequestFail = "Ошибка отправки через протокол {0} : {1}";
        public const string PingRequestStop = "Ping остановлен";

        public const string TcpIpPortPattern = @"^([1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9])[:](\d+)$";

        public const string TcpIpCheckPattern = @"^[1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9]$";

        public const string HttpUrlOrIpCheckPattern = @"^http[s]?[:]//(?:(?:\w+(?:[-.]?\w+)*[.][a-zA-Z]+)|(?:[1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9]))(?:[:]\d+)?(?:[/].*)?$";

        public const string IcmpUrlOrIpCheckPattern = @"(?:^\w+(?:[-.]?\w+)*[.][a-zA-Z]+$)|(?:^[1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9][.][1-2]?[0-9]?[0-9]$)";
    }
}
