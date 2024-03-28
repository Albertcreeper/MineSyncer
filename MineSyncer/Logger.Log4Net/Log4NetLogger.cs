using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;

namespace Logger.Log4Net
{
    public class Log4NetLogger<T> : ILogger<T>
    {
        protected log4net.ILog _Logger;

        public Log4NetLogger()
        {
            _Logger = log4net.LogManager.GetLogger(typeof(T));
        }

        public void LogDebug(string message)
        {
            _Logger.Debug(message);
        }

        public void LogDebug(string message, params object[] args)
        {
            _Logger.Debug(String.Format(message, args));
        }

        public void LogError(string message)
        {
            _Logger.Error(message);
        }

        public void LogError(string message, params object[] args)
        {
            _Logger.Error(String.Format(message, args));
        }

        public void LogError(Exception ex, string message)
        {
            _Logger.Error(message, ex);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _Logger.Error(String.Format(message, args), ex);
        }

        public void LogFatal(string message)
        {
            _Logger.Fatal(message);
        }

        public void LogFatal(string message, params object[] args)
        {
            _Logger.Fatal(String.Format(message, args));
        }

        public void LogFatal(Exception ex, string message)
        {
            _Logger.Fatal(message, ex);
        }

        public void LogFatal(Exception ex, string message, params object[] args)
        {
            _Logger.Fatal(String.Format(message, args), ex);
        }

        public void LogInfo(string message)
        {
            _Logger.Info(message);
        }

        public void LogInfo(string message, params object[] args)
        {
            _Logger.Info(String.Format(message, args));
        }

        public void LogWarn(string message, params object[] args)
        {
            _Logger.Warn(String.Format(message, args));
        }

        public void LogWarn(Exception ex, string message)
        {
            _Logger.Warn(message, ex);
        }

        public void LogWarn(Exception ex, string message, params object[] args)
        {
            _Logger.Warn(String.Format(message, args), ex);
        }
    }
}
