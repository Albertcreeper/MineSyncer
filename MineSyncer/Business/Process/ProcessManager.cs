using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Process
{
    public class ProcessManager : IProcessManager
    {
        protected static int _WATCH_INTERVAL = 2500;

        public System.Diagnostics.Process GetRunningProcessByName(string proccessName)
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(proccessName);

            if (processes.Length == 0)
            {
                throw new ApplicationException($"Laufener Prozess '{proccessName}' nicht gefunden");
            }

            if (processes.Length > 1)
            {
                throw new ApplicationException($"Zu viele Prozesse laufen unter dem Namen '{proccessName}'");
            }

            return processes[0];
        }

        public System.Diagnostics.Process StartProcess(string process)
        {
            return System.Diagnostics.Process.Start(process);
        }

        public System.Diagnostics.Process StartProcess(string process, params string[] arguments)
        {
            return System.Diagnostics.Process.Start(process, arguments);
        }

        public async Task WaitForExit(System.Diagnostics.Process process, Action actionToRun)
        {
            await Task.Run(() =>
            {
                do
                {
                    if(process.HasExited)
                    {
                        actionToRun.Invoke();
                        return;
                    }

                    Task.Delay(_WATCH_INTERVAL);
                } 
                while (true);
            });
        }

        public async Task<System.Diagnostics.Process> WatchForProcessByName(string process)
        {
            return await Task.Run(() =>
            {
                do
                {
                    var resProcess = System.Diagnostics.Process.GetProcessesByName(process);
                    if(resProcess.Length > 1)
                    {
                        throw new ApplicationException($"Zu viele Prozesse laufen unter dem Namen '{process}'");
                    }
                    else if(resProcess.Length == 1)
                    {
                        return resProcess[0];
                    }

                    Task.Delay(_WATCH_INTERVAL);
                }
                while (true);
            });
        }

        public async Task<System.Diagnostics.Process> WatchForProcessByName(string process, Action<System.Diagnostics.Process> actionToRun)
        {
            var resProcess = await WatchForProcessByName(process);
            actionToRun.Invoke(resProcess);

            return resProcess;
        }
    }
}
