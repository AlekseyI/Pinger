using Pinger.Enums;
using Pinger.Exceptions;
using Pinger.Log;
using Pinger.Log.TextFile;
using System;

namespace Pinger.Factory
{
    public class LogFactory : IFactory<ILogInput, ILog>
    {
        public ILog GetInstance(ILogInput logInput)
        {
            if (logInput == null)
            {
                throw new LogException(nameof(logInput), nameof(ArgumentNullException));
            }

            switch (logInput.Format)
            {
                case LogFormatEnum.TextFile:
                    return new LogTextFile(logInput);
                default:
                    throw new LogException($"{nameof(logInput.Format)} = {logInput.Format} is unsupported", nameof(ArgumentException));
            }
        }
    }
}
