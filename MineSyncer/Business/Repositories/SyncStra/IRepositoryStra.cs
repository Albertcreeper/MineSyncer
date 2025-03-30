using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra
{
    public interface IRepositoryStra
    {
        DateTime GetLastChangeOfRepository(IRepository repository);

        void Create(IRepository repository);

        void CreateBackupOfRepository(IRepository repository);

        void CreateBackupOfRepository(IRepository repository, string name);

        void CreateBackupOfRepository(IRepository repository, string name, string path);

        string GetNextBackUpName(IRepository repository);

        bool Exists(IRepository repository);

        bool IsEmpty(IRepository repository);

        void PushToRemoteBefore(ILocalRepository repository, IRemoteRepository remoteRepository);

        void PushToRemote(ILocalRepository repoistory, IRemoteRepository remoteRepository);

        void PushToRemoteAction(ILocalRepository repository, IRemoteRepository remoteRepository);

        void PushToRemoteAfter(ILocalRepository repository, IRemoteRepository remoteRepository);

        void PullFromRemoteBefore(ILocalRepository repository, IRemoteRepository remoteRepository);

        void PullFromRemote(ILocalRepository repository, IRemoteRepository remoteRepository);

        void PullFromRemoteAction(ILocalRepository repository, IRemoteRepository remoteRepository);

        void PullFromRemoteAfter(ILocalRepository repository, IRemoteRepository remoteRepository);
    }
}
