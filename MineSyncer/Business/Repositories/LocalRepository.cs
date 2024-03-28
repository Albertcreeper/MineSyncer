using Business.Core;
using Business.Files;
using Business.Repositories.Extensions;
using Business.Repositories.SyncStra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories
{
    public class LocalRepository : Repository, ILocalRepository
    {
        public string ProcessName { get; set; }
        public string StartCommandLine { get; set; }
        public string RemoteRepositoryConfigPath { get; set; }
        public IRemoteRepository RemoteRepository { get; set; }

        public LocalRepository(string name, string configPath) : base(name, configPath)
        {
            
        }

        public void PullFromRemote()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _Stra.PullFromRemote(this, RemoteRepository);

            stopwatch.Stop();
            _Logger.LogInfo($"Finished synchronisation after <{stopwatch.ElapsedMilliseconds}> milliseconds");
        }

        public void PushToRemote()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            _Stra.PushToRemote(this, RemoteRepository);

            stopwatch.Stop();
            _Logger.LogInfo($"Finished synchronisation after <{stopwatch.ElapsedMilliseconds}> milliseconds");
        }

        public override void Refresh()
        {
            _Logger.LogDebug($"Refresh local repository <{Name}>");
            base.Refresh();

            // local repository
            ProcessName = _ConfigService.Get<string>("ProcessName");
            StartCommandLine = _ConfigService.Get<string>("StartCommandLine");
            Variables = ConvertToVariables(_ConfigService.Get<string>("Variables"));

            // remote repository
            RemoteRepositoryConfigPath = _ConfigService.Get<string>("RemoteRepositoryConfigPath");
            RemoteRepository = new RemoteRepository(Name, RemoteRepositoryConfigPath, Variables);
            RemoteRepository.Refresh();

            _Logger.LogDebug($"Repository {Name}: {ToString()}");
        }

        public override void Save()
        {
            base.Save();

            _ConfigService.Set("ProcessName", ProcessName);
            _ConfigService.Set("StartCommandLine", StartCommandLine);
            _ConfigService.Set("RemoteRepositoryConfigPath", RemoteRepositoryConfigPath);
        }

        public void Synchronize()
        {
            switch(GetSynchronizePreview())
            {
                case SyncPreview.PushToRemote:
                    PushToRemote();
                    break;
                case SyncPreview.PullFromRemote:
                    PullFromRemote();
                    break;
                default:
                    break;
            }
        }

        public SyncPreview GetSynchronizePreview()
        {
            ValidateVersion();

            if(IsNewerThanRemote())
            {
                return SyncPreview.PushToRemote;
            }
            else if(IsOlderThanRemote())
            {
                return SyncPreview.PullFromRemote;
            }

            return SyncPreview.Nothing;
        }

        public void ValidateVersion()
        {
            Refresh();

            if((Version >= RemoteRepository.Version &&
                LastChange >= RemoteRepository.LastChange) ||
                    (RemoteRepository.Version >= Version &&
                RemoteRepository.LastChange >= LastChange))
            {
                return;
            }

            throw new ApplicationException("Could not compare repositories. Version and dates do not match.");
        }

        public bool IsNotNewerThanRemote()
        {
            return IsNotNewerThan(RemoteRepository);
        }

        public bool IsNewerThanRemote()
        {
            return IsNewerThan(RemoteRepository);
        }

        public bool IsNotOlderThanRemote()
        {
            return IsNotOlderThan(RemoteRepository);
        }

        public bool IsOlderThanRemote()
        {
            return IsOlderThan(RemoteRepository);
        }

        public override string ToString()
        {
            return $"Name={Name}|ConfigPath={ConfigPath}|Path={Path}|RemoteRepositoryConfigPath ={RemoteRepositoryConfigPath}|" +
                $"RemoteRepository={RemoteRepository}|CreateBackup={CreateBackup}|BackupPath={BackupPath}|" +
                $"Version={Version}|LastChange={LastChange}|ProcessName={ProcessName}|StartCommandLine={StartCommandLine}|Variables={ConvertToString(Variables)}|Stra={_Stra.ToString()}|";
        }
    }
}
