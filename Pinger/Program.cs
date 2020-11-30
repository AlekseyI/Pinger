using Pinger.Config;
using Pinger.Enums;
using Pinger.Factory;
using Pinger.Factory.Ping;
using Pinger.Logger;
using Pinger.Manager;
using System;

namespace Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            var pingRequestFactory = new PingRequestFactory();
            var configSource = new ConfigSource(Constant.Config, ConfigFormat.JsonFile);
            var logSource = new LogSource(Constant.Log, LogFormat.TextFile);
            var config = new ConfigFactory().GetInstance(configSource);
            var log = new LogFactory().GetInstance(logSource);
            var configData = new DefaultConfigFactory().GetInstance(configSource);
            
            using (var pinger = new PingManager(pingRequestFactory, config, log))
            {
                if (pinger.CheckConfig(configData))
                {
                    pinger.Start(false);
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine(Constant.Config + " не найден, поэтому он был создан");
                }
            }
            Console.ReadKey();
        }
    }
}