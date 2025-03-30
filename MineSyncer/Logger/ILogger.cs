using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public interface ILogger<T>
    {
        void LogDebug(string message);

        void LogDebug(string message, params object[] args);

        void LogInfo(string message);

        void LogInfo(string message, params object[] args);

        void LogWarn(string message, params object[] args);

        void LogWarn(Exception ex, string message);

        void LogWarn(Exception ex, string message, params object[] args);

        void LogError(string message);

        void LogError(string message, params object[] args);

        void LogError(Exception ex, string message);

        void LogError(Exception ex, string message, params object[] args);

        void LogFatal(string message);

        void LogFatal(string message, params object[] args);

        void LogFatal(Exception ex, string message);

        void LogFatal(Exception ex, string message, params object[] args);
    }
}
