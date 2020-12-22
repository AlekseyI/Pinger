using System;
using System.IO;
using System.Threading.Tasks;

namespace Pinger.Logger.TextFile
{
    public class LoggerTextFile : ILogger<ILogSource, ILogData>
    {
        public ILogSource LogSource { get; }

        public LoggerTextFile(ILogSource logSource)
        {
            if (logSource == null)
            {
                throw new ArgumentNullException(nameof(logSource));
            }

            LogSource = logSource;
        }

        public async Task WriteAsync(ILogData logData)
        {
            using (var stream = new StreamWriter(LogSource.Path, true))
            {
                await stream.WriteLineAsync(logData.Log.ToString());
            }
        }
    }
}