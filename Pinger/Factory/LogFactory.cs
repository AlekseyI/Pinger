using Pinger.Enums;
using Pinger.Logger;
using Pinger.Logger.TextFile;
using System;

namespace Pinger.Factory
{
    public class LogFactory : IFactory<ILogSource, ILogger<ILogSource, ILogData>>
    {
        public ILogger<ILogSource, ILogData> GetInstance(ILogSource loggerSource)
        {
            if (loggerSource == null)
            {
                throw new ArgumentNullException(nameof(loggerSource));
            }

            switch (loggerSource.Format)
            {
                case LogFormat.TextFile:
                    return new LoggerTextFile(loggerSource);
                default:
                    throw new ArgumentException($"{nameof(loggerSource.Format)} = {loggerSource.Format} is unsupported");
            }
        }
    }
}
