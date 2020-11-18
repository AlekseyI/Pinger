using Pinger.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pinger.Log.TextFile
{
    public class LogTextFile : ILog
    {
        public ILogInput LogInput { get; }

        public LogTextFile(ILogInput logInput)
        {
            if (logInput == null)
            {
                throw new LogException(nameof(logInput), nameof(ArgumentNullException));
            }

            LogInput = logInput;
        }

        public void Write(ILogData logData)
        {
            if (logData == null)
            {
                throw new LogException(nameof(logData), nameof(ArgumentNullException));
            }

            StreamWriter stream = null;
            try
            {
                using (stream = new StreamWriter(LogInput.Path, true))
                {
                    stream.WriteLine(logData.Log.FormatToString());
                }
            }
            catch (IOException e)
            {
                stream?.Close();
                throw new LogException(e.Message, e.GetType().Name);
            }
            catch (ArgumentException e)
            {
                stream?.Close();
                throw new LogException(e.Message, e.GetType().Name);
            }
            catch (SystemException e)
            {
                stream?.Close();
                throw new LogException(e.Message, e.GetType().Name);
            }
        }

        public async Task WriteAsync(ILogData logData)
        {
            if (logData == null)
            {
                throw new LogException(nameof(logData), nameof(ArgumentNullException));
            }

            StreamWriter stream = null;
            try
            {
                using (stream = new StreamWriter(LogInput.Path, true))
                {
                    await stream.WriteLineAsync(logData.Log.FormatToString());
                }
            }
            catch (IOException e)
            {
                stream?.Close();
                throw new LogException(e.Message, e.GetType().Name);
            }
            catch (ArgumentException e)
            {
                stream?.Close();
                throw new LogException(e.Message, e.GetType().Name);
            }
            catch (SystemException e)
            {
                stream?.Close();
                throw new LogException(e.Message, e.GetType().Name);
            }

        }
    }
}