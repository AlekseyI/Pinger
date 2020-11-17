using Pinger.Enums;
using Pinger.Manager;
using System;

namespace Pinger
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var pinger = new PingerManager(ConfigFormatEnum.JsonFile, LogFormatEnum.TextFile))
            {
                pinger.EventStatus += Status;
                if (pinger.CheckConfig())
                {
                    pinger.Start();
                    Console.ReadKey();
                }
            }
        }

        public static void Status(string value)
        {
            Console.WriteLine(value);
        }
    }
}