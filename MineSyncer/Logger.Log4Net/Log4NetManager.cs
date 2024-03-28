using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Logger.Log4Net
{
    public class Log4NetManager : ILogManager
    {
        string _ConfigPath = "";

        public Log4NetManager()
        {

        }

        public void ConfigureLogging(string configFilePath)
        {
            _ConfigPath = configFilePath;

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(_ConfigPath));
        }

        public void ConfigureLogging()
        {
            ConfigureLogging(_ConfigPath);
        }

        public ILogger<T> GetLogger<T>()
        {
            return new Log4NetLogger<T>();
        }
    }
}
