using Pinger.Config;
using Pinger.Enums;
using Pinger.Factory;
using Pinger.Factory.Ping;
using Pinger.Log;
using Pinger.Manager;
using System;

namespace Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            var pingRequestFactory = new PingRequestFactory();
            var configInput = new ConfigInput(Constant.Config, ConfigFormatEnum.JsonFile);
            var logInput = new LogInput(Constant.Log, LogFormatEnum.TextFile);
            var config = new ConfigFactory().GetInstance(configInput);
            var log = new LogFactory().GetInstance(logInput);
            var configData = new DefaultConfigFactory().GetInstance(configInput);
            
            using (var pinger = new PingManager(pingRequestFactory, config, log))
            {
                pinger.EventStatus += Status;
                if (pinger.CheckConfig(configData))
                {
                    pinger.Start();
                    Console.ReadKey();
                }
            }
            Console.ReadKey();
        }

        public static void Status(string value)
        {
            Console.WriteLine(value);
        }
    }
}