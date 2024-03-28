using Business.Core;
using Business.Files;
using Business.Repositories.SyncStra.File;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra.OneDrive
{
    public class OneDriveRepoStra : RepositoryStraKernel
    {
        private static ILogger<OneDriveRepoStra> _Logger;

        public static readonly string Name = "OneDrive";

        public OneDriveRepoStra()
        {
            _FileService = BaseFactory.GetFactory().GetService<IFileService>();
            _BackupService = BaseFactory.GetFactory().GetService<IBackupService>();
            _Logger = BaseFactory.GetFactory().GetService<ILogManager>().GetLogger<OneDriveRepoStra>();
        }
    }
}
