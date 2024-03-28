using Business.Core;
using Business.Repositories.Extensions;
using Business.Repositories.SyncStra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories
{
    public class RemoteRepository : Repository, IRemoteRepository
    {
        ISyncStra _SyncStra { get; set; }

        public RemoteRepositoryType RepositoryType { get; protected set; }

        public RemoteRepository(string name, string configPath) : this(name, configPath, null)
        {

        }

        public RemoteRepository(string name, string configPath, Dictionary<string, string> variables) : base(name, configPath)
        {
            Variables = variables;
        }
          
        public override void Refresh()
        {
            base.Refresh();
        }

        public override void AfterRefresh()
        {
            base.AfterRefresh();

            string repositoryType = _ConfigService.Get<string>("RepositoryType");
            RepositoryType = ConvertToRemoteRepositoryType(repositoryType);

            _Stra = BaseFactory.GetFactory().GetStra(repositoryType);
            _SyncStra = BaseFactory.GetFactory().GetSyncStra(repositoryType);
            ResolvePaths();
        }

        public override void Save()
        {
            _ConfigService.Set("Name", Name);
            //_InitService.Set(ConfigPath, Name, "Path", Path);
            //_InitService.Set(ConfigPath, Name, "BackupPath", BackupPath);
            _ConfigService.Set("CreateBackup", CreateBackup);
            _ConfigService.Set("Version", Version);
        }

        public void ResolvePaths()
        {
            if(Variables is null)
            {
                _Logger.LogDebug("Variables is null, nothing to replace");
                return;
            }

            _Logger.LogDebug("Resolve paths by variables: " + ConvertToString(Variables));
            
            foreach(KeyValuePair<string, string> keyValue in Variables)
            {
                _Logger.LogDebug($"Replace placeholder: {keyValue.Key} => {keyValue.Value}");
                Path = Path.Replace(keyValue.Key, keyValue.Value);
                BackupPath = BackupPath.Replace(keyValue.Key, keyValue.Value);
            }
        }

        public ISyncStra GetSyncStra()
        {
            return _SyncStra;
        }

        protected static RemoteRepositoryType ConvertToRemoteRepositoryType(string type)
        {
            if (type.Equals("OneDrive"))
            {
                return RemoteRepositoryType.OneDrive;
            }
            else if (type.Equals("File"))
            {
                return RemoteRepositoryType.File;
            }

            return RemoteRepositoryType.None;
        }
    }
}
