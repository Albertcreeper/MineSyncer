using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public interface ILogManager
    {
        void ConfigureLogging();

        ILogger<T> GetLogger<T>();
    }
}
