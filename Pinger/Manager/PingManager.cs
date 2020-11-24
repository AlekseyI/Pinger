using Pinger.Config;
using Pinger.Exceptions;
using Pinger.Factory;
using Pinger.Input;
using Pinger.Log;
using Pinger.Request;
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
        private IConfig _config;
        private ILog _log;
        private IFactory<IConfigData, IPing<IHostInput, IPingResponse>> _pingRequestFactory;
        private Task[] _tasksPing;

        private CancellationTokenSource _sourceToken;
        private readonly object _locker;

        public delegate void Status(string value);
        public event Status EventStatus;

        public PingManager(IFactory<IConfigData, IPing<IHostInput, IPingResponse>> pingRequestFactory, IConfig config, ILog log)
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

        public void Start()
        {
            try
            {
                lock (_locker)
                {
                    if (_config == null || _configData != null)
                    {
                        return;
                    }

                    _configData = _config.Read();

                    EventStatus?.Invoke(string.Format(Constant.ConfigSuccessRead, Constant.Config));

                    _sourceToken?.Dispose();
                    _sourceToken = new CancellationTokenSource();

                    _tasksPing = new Task[_configData.Count()];

                    EventStatus?.Invoke(Constant.PingRequestStart);

                    int i = 0;
                    foreach (var config in _configData)
                    {
                        _tasksPing[i] = SendPing(config, _sourceToken.Token);
                        i++;
                    }
                }
            }
            catch (ConfigException e)
            {
                EventStatus?.Invoke(string.Format(Constant.ConfigFailRead, Constant.Config, e));
                _configData = null;
            }
        }

        private async Task SendPing(IConfigData configData, CancellationToken token)
        {
            IPing<IHostInput, IPingResponse> pingRequest;

            try
            {
                pingRequest = _pingRequestFactory.GetInstance(configData);
            }
            catch (PingRequestException e)
            {
                EventStatus?.Invoke(string.Format(Constant.PingRequestInstanceFail, configData.Protocol, e));
                return;
            }

            var logData = new LogData();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    logData.Log = pingRequest.Ping();
                    await _log.WriteAsync(logData);
                }
                catch (PingRequestException e)
                {
                    EventStatus?.Invoke(string.Format(Constant.PingRequestFail, configData.Protocol, e));
                    break;
                }
                catch (LogException e)
                {
                    EventStatus?.Invoke(string.Format(Constant.LogFailWrite, Constant.Log, e));
                    break;
                }

                try
                {
                    await Task.Delay(configData.Period, token);
                }
                catch(TaskCanceledException)
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
                EventStatus?.Invoke(Constant.PingRequestStop);
            }
        }

        public bool CheckConfig(IConfigData configData)
        {
            if (configData == null)
            {
                throw new ArgumentNullException(nameof(configData));
            }

            try
            {
                lock (_locker)
                {
                    if (_config.CreateDefaultConfig(configData))
                    {
                        EventStatus?.Invoke(string.Format(Constant.ConfigNotFoundButCreated, Constant.Config));
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (ConfigException e)
            {
                EventStatus?.Invoke(string.Format(Constant.ConfigFailCreate, Constant.Config, e));
                return false;
            }
        }

        public void Dispose()
        {
            Stop();
            _sourceToken?.Dispose();
            _sourceToken = null;

            var lstDlg = EventStatus?.GetInvocationList();

            if (lstDlg != null)
            {
                foreach (var dlg in lstDlg)
                {
                    EventStatus -= (Status)dlg;
                }
            }
        }
    }
}