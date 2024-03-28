using Business.Core;
using Business.Core.Exceptions;
using Business.Files;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra
{
    public class RepositoryStraKernel : IRepositoryStra
    {
        protected IFileService _FileService;
        protected IBackupService _BackupService;
        private static ILogger<RepositoryStraKernel> _Logger;

        public RepositoryStraKernel()
        {
            _FileService = BaseFactory.GetFactory().GetService<IFileService>();
            _BackupService = BaseFactory.GetFactory().GetService<IBackupService>();
            _Logger = BaseFactory.GetFactory().GetService<ILogManager>().GetLogger<RepositoryStraKernel>();
        }

        public virtual void Create(IRepository repository)
        {
            if (String.IsNullOrEmpty(repository.Name))
            {
                throw new AppException("Name must not be empty!");
            }

            if (String.IsNullOrEmpty(repository.Path))
            {
                throw new AppException("Path must not be empty!");
            }

            _FileService.CreateFolder(repository.Path);
        }

        public virtual void CreateBackupOfRepository(IRepository repository)
        {
            CreateBackupOfRepository(repository, GetNextBackUpName(repository), repository.BackupPath);
        }

        public virtual void CreateBackupOfRepository(IRepository repository, string name)
        {
            CreateBackupOfRepository(repository, name, repository.BackupPath);
        }

        public virtual void CreateBackupOfRepository(IRepository repository, string name, string path)
        {
            _Logger.LogInfo($"Create backup <{name} of repository <{repository.Name}> at location <{path}>");
            _BackupService.CreateBackup(repository.Path, $"{path}\\{name}");
        }

        public virtual string GetNextBackUpName(IRepository repository)
        {
            string backupName = repository.Name + "_" + repository.Version + "_" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss");

            if (_FileService.ExistsFile($"{repository.BackupPath}\\{backupName}"))
            {
                int backupNumber = 2;

                while (_FileService.ExistsFile($"{repository.BackupPath}\\{backupName}_{backupNumber}"))
                {
                    backupNumber += 1;
                }

                backupName += "_" + backupNumber;
            }

            return backupName;
        }

        public virtual bool Exists(IRepository repository)
        {
            if (String.IsNullOrEmpty(repository.Path))
            {
                throw new AppException("Path must not be empty!");
            }

            return _FileService.ExistsFolder(repository.Path);
        }

        public virtual bool IsEmpty(IRepository repository)
        {
            if (String.IsNullOrEmpty(repository.Path))
            {
                throw new AppException("Path must not be empty!");
            }

            return _FileService.IsFolderEmpty(repository.Path);
        }

        public virtual void PushToRemote(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            _Logger.LogInfo($"Synchronize worlds (PUSH to REMOTE): Push from <{repository.Path}> to remote <{remoteRepository.Path}> to update remote worlds ");

            // before
            PushToRemoteBefore(repository, remoteRepository);

            // push to remote
            PushToRemoteAction(repository, remoteRepository);

            // after
            PushToRemoteAfter(repository, remoteRepository);
        }

        public virtual void PushToRemoteBefore(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            // refresh
            _Logger.LogDebug("Refresh data");
            repository.Refresh();

            // check if exists remote repository. Create new folder if not exists.
            if (!remoteRepository.Exists())
            {
                _Logger.LogDebug($"Remote path <{repository.Path}> not found. Create new folder..");
                remoteRepository.Create();
            }

            // check if remote repository exists
            if (!repository.Exists())
            {
                _Logger.LogError($"Local path <{repository.Path}> not found, nothing to update.");
                throw new ApplicationException("Repository not found");
            }

            // check if remote repository is empty
            if (repository.IsEmpty())
            {
                _Logger.LogError($"Repository <{repository.Path}> contains no files, nothing to update.");
                throw new ApplicationException("Repository contains no files");
            }

            // check data consistency
            _Logger.LogInfo("Validate version");
            repository.ValidateVersion();

            // compare versions
            if (repository.IsOlderThanRemote())
            {
                throw new ApplicationException("Conflict detected. Remote version is newer than local!");
            }

            // create backup
            if (remoteRepository.CreateBackup && !String.IsNullOrEmpty(remoteRepository.BackupPath))
            {
                remoteRepository.CreateBackupOfRepository();
            }
        }

        public virtual void PushToRemoteAction(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            // copy world
            _Logger.LogDebug($"Start synchronisation between local path <{repository.Path}> and remote path <{remoteRepository.Path}>");
            remoteRepository.GetSyncStra().PushToRemote(repository, remoteRepository);
        }

        public virtual void PushToRemoteAfter(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            // get next version
            _Logger.LogDebug("Get next version");
            int nextVersion = GetNextVersion(repository);

            // set new version
            repository.Version = nextVersion;
            repository.Save();

            remoteRepository.Version = nextVersion;
            remoteRepository.Save();
        }

        public virtual void PullFromRemote(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            _Logger.LogInfo($"Synchronize worlds (PULL from REMOTE): Pull from remote <{remoteRepository.Path}> to update local worlds <{repository.Path}>");

            // before
            PullFromRemoteBefore(repository, remoteRepository);

            // pull from remote
            PullFromRemoteAction(repository, remoteRepository);

            // after
            PullFromRemoteAfter(repository, remoteRepository);
        }

        public virtual void PullFromRemoteBefore(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            // refresh
            _Logger.LogDebug("Refresh data");
            repository.Refresh();

            // check if local path exists, if not, so create new folder
            if (!repository.Exists())
            {
                _Logger.LogDebug($"Local path <{repository.Path}> not found. Create new folder..");
                repository.Create();
            }

            // check if remote repository exists
            if (!remoteRepository.Exists())
            {
                _Logger.LogError($"Remote path <{remoteRepository.Path}> not found, nothing to update.");
                throw new ApplicationException("Remote repository not found");
            }

            // check if remote repository is empty
            if (remoteRepository.IsEmpty())
            {
                _Logger.LogError($"Remote repository <{remoteRepository.Path}> contains no files, nothing to update.");
                throw new ApplicationException("Remote repository contains no files");
            }

            // check data consistency
            _Logger.LogInfo("Validate version");
            repository.ValidateVersion();

            // compare versions
            if (repository.IsNewerThanRemote())
            {
                throw new ApplicationException("Conflict detected. Local version is newer than remote!");
            }

            // create backup
            if (repository.CreateBackup && !String.IsNullOrEmpty(repository.BackupPath))
            {
                CreateBackupOfRepository(repository);
            }
        }

        public virtual void PullFromRemoteAction(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            // pull from remote
            _Logger.LogDebug($"Start synchronization between local path <{repository.Path}> and remote path <{remoteRepository.Path}>");
            remoteRepository.GetSyncStra().PullFromRemote(repository, remoteRepository);
        }

        public virtual void PullFromRemoteAfter(ILocalRepository repository, IRemoteRepository remoteRepository)
        {
            // set version
            repository.Version = remoteRepository.Version;
            _Logger.LogDebug("Save data");
            repository.Save();
        }

        protected virtual int GetNextVersion(IRepository repository)
        {
            return repository.Version + 1;
        }

        public DateTime GetLastChangeOfRepository(IRepository repository)
        {
            return _FileService.GetLastChangeOfDirectory(repository.Path);
        }
    }
}
