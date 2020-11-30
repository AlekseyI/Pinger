using Pinger.Config;
using Pinger.Factory;
using Pinger.Input;
using Pinger.Logger;
using Pinger.Connection;
using Pinger.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pinger.Manager
{
    public class PingManager : IPingManager
    {
        private IEnumerable<IConfigData> _configData;
        private IConfig<IConfigSource, IEnumerable<IConfigData>> _config;
        private ILogger<ILogSource, ILogData> _log;
        private IFactory<IConfigData, IPing<IHostInput, IPingResponse>> _pingRequestFactory;
        private Task[] _tasksPing;

        private CancellationTokenSource _sourceToken;
        private readonly object _locker;

        public PingManager(IFactory<IConfigData, IPing<IHostInput, IPingResponse>> pingRequestFactory, IConfig<IConfigSource, IEnumerable<IConfigData>> config, ILogger<ILogSource, ILogData> log)
        {
            if (pingRequestFactory == null)
            {
                throw new ArgumentNullException(nameof(pingRequestFactory));
            }
            else if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            else if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            else
            {
                _pingRequestFactory = pingRequestFactory;
                _config = config;
                _log = log;
                _locker = new object();
            }
        }

        public void Start(bool isWait)
        {
            lock (_locker)
            {
                if (_config == null || _configData != null)
                {
                    return;
                }

                _configData = _config.Read();

                _sourceToken?.Dispose();
                _sourceToken = new CancellationTokenSource();

                _tasksPing = new Task[_configData.Count()];

                int i = 0;
                foreach (var config in _configData)
                {
                    _tasksPing[i] = SendPing(config, _sourceToken.Token);
                    i++;
                }
            }

            if (isWait)
            {
                Task.WaitAll(_tasksPing);
            }
        }

        private async Task SendPing(IConfigData configData, CancellationToken token)
        {
            IPing<IHostInput, IPingResponse> pingRequest;

            pingRequest = _pingRequestFactory.GetInstance(configData);

            var logData = new LogData();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await pingRequest.Ping();
                    logData.Log = pingRequest.Response;
                    await _log.WriteAsync(logData);
                }
                catch (Exception)
                {
                    break;
                }

                try
                {
                    await Task.Delay(configData.Period, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            pingRequest.Dispose();
        }

        public void Stop()
        {
            lock (_locker)
            {
                _sourceToken?.Cancel();

                if (_tasksPing != null)
                {
                    Task.WaitAll(_tasksPing);
                }

                _tasksPing = null;
                _configData = null;
            }
        }

        public bool CheckConfig(IConfigData configData)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            lock (_locker)
            {
                if (_config.CreateDefaultConfig(new IConfigData[] { configData }))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void Dispose()
        {
            Stop();
            _sourceToken?.Dispose();
            _sourceToken = null;
        }
    }
}