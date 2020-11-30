using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pinger.Enums;

namespace Pinger.Config.JsonFile
{
    public class ConfigJsonData : IConfigData
    {
        public string Host { get; set; }

        public TimeSpan Period { get; set; }

        public TimeSpan TimeOut { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TypeProtocol Protocol { get; set; }
    }
}