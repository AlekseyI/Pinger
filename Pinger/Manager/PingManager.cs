using Pinger.Config;
using Pinger.Enums;
using Pinger.Exceptions;
using Pinger.Factory;
using Pinger.Factory.Ping;
using Pinger.Input;
using Pinger.Log;
using Pinger.Request;
using Pinger.Response;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pinger.Manager
{
    public class PingerManager : IPingManager
    {
        private IEnumerable<IConfigData> _configData;
        private IConfig _config;
        private ILog _log;
        private IFactory<IConfigData, IPing<IHostInput, IPingResponse>> _pingRequestFactory;
        private readonly ConfigFormatEnum _configFormat;

        private CancellationTokenSource _sourceToken;
        private readonly object _locker;

        public delegate void Status(string value);
        public event Status EventStatus;

        public PingerManager(ConfigFormatEnum configFormat, LogFormatEnum logFormat)
        {
            _configFormat = configFormat;
            _config = new ConfigFactory().GetInstance(new ConfigInput(Constant.Config, configFormat));
            _log = new LogFactory().GetInstance(new LogInput(Constant.Log, logFormat));
            _pingRequestFactory = new PingRequestFactory();
            _locker = new object();
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

                    EventStatus?.Invoke(Constant.PingRequestStart);

                    foreach (var config in _configData)
                    {
                        SendPing(config, _sourceToken.Token);
                    }
                }
            }
            catch (ConfigException e)
            {
                EventStatus?.Invoke(string.Format(Constant.ConfigFailRead, Constant.Config, e));
                _configData = null;
            }
        }

        private async void SendPing(IConfigData configData, CancellationToken token)
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

                await Task.Delay(configData.Period);
            }

            pingRequest.Dispose();
        }

        public void Stop()
        {
            lock (_locker)
            {
                _sourceToken?.Cancel();
                _configData = null;
                EventStatus?.Invoke(Constant.PingRequestStop);
            }
        }

        public bool CheckConfig()
        {
            try
            {
                lock (_locker)
                {
                    if (_config.CreateDefaultConfig(new DefaultConfigFactory().GetInstance(new ConfigInput(Constant.Config, _configFormat))))
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
            _pingRequestFactory = null;
            _config = null;
            _log = null;

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