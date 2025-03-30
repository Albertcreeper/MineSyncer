using Business.Core;
using Business.Files;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra.File
{
    public class FileSyncStra : ISyncStra
    {
        protected IFileService _FileService;

        public static readonly string Name = "File";

        public FileSyncStra()
        {
            _FileService = BaseFactory.GetFactory().GetService<IFileService>();
        }

        public void PullFromRemote(ILocalRepository localRepository, IRemoteRepository remoteRepository)
        {
            _FileService.Copy(remoteRepository.Path, localRepository.Path, CopyOption.CopyOnlyChanges);
        }

        public void PushToRemote(ILocalRepository localRepository, IRemoteRepository remoteRepository)
        {
            _FileService.Copy(localRepository.Path, remoteRepository.Path, CopyOption.CopyOnlyChanges);
        }
    }
}
