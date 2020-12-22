using Pinger.Config;
using Pinger.Factory;
using Pinger.Input;
using Pinger.Logger;
using Pinger.Connection;
using Pinger.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
        private bool _isStarted;

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
            if (_isStarted)
            {
                return;
            }

            lock (_locker)
            {
                _configData = _config.Read();

                _sourceToken = new CancellationTokenSource();

                var config = _configData.ToArray();
                _tasksPing = new Task[config.Length];

                for (int i = 0; i < config.Length; i++)
                {
                    var pingRequest = _pingRequestFactory.GetInstance(config[i]);
                    _tasksPing[i] = SendPing(pingRequest, config[i].Period, _sourceToken.Token);
                }

                _isStarted = true;
            }

            if (isWait)
            {
                Task.WaitAll(_tasksPing);
            }
        }

        private async Task SendPing(IPing<IHostInput, IPingResponse> pingRequest, TimeSpan period, CancellationToken token)
        {
            using(pingRequest)
            {
                var logData = new LogData();

                while (!token.IsCancellationRequested)
                {
                    await pingRequest.Ping();
                    logData.Log = pingRequest.Response;
                    await _log.WriteAsync(logData);

                    try
                    {
                        await Task.Delay(period, token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        public void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            lock (_locker)
            {
                _sourceToken?.Cancel();

                if (_tasksPing != null)
                {
                    foreach (var task in _tasksPing)
                    {
                        task?.Wait();
                    }
                }

                _sourceToken?.Dispose();
                _sourceToken = null;
                _tasksPing = null;
                _configData = null;
                _isStarted = false;
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
                return !_config.CreateDefaultConfig(new IConfigData[] { configData });
            }
        }
    }
}