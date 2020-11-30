using System;
using System.IO;
using System.Threading.Tasks;

namespace Pinger.Logger.TextFile
{
    public class LoggerTextFile : ILogger<ILogSource, ILogData>
    {
        public ILogSource LoggerSource { get; }

        public LoggerTextFile(ILogSource loggerSource)
        {
            if (loggerSource == null)
            {
                throw new ArgumentNullException(nameof(loggerSource));
            }

            LoggerSource = loggerSource;
        }

        public async Task WriteAsync(ILogData loggerData)
        {
            if (loggerData == null)
            {
                throw new ArgumentNullException(nameof(loggerData));
            }

            using (var stream = new StreamWriter(LoggerSource.Path, true))
            {
                await stream.WriteLineAsync(loggerData.Log.FormatToString());
            }
        }
    }
}