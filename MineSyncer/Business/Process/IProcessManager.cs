using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Process
{
    public interface IProcessManager
    {
        System.Diagnostics.Process StartProcess(string process);

        System.Diagnostics.Process StartProcess(string process, params string[] arguments);

        System.Diagnostics.Process GetRunningProcessByName(string proccessName);

        Task<System.Diagnostics.Process> WatchForProcessByName(string process);

        Task<System.Diagnostics.Process> WatchForProcessByName(string process, Action<System.Diagnostics.Process> actionToRun);

        Task WaitForExit(System.Diagnostics.Process process, Action actionToRun);
    }
}
