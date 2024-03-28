using Business.Core;
using Business.Core.Exceptions;
using Business.Files;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra.File
{
    public class FileRepositoryStra : RepositoryStraKernel
    {
        private static ILogger<FileRepositoryStra> _Logger;

        public static readonly string Name = "File";

        public FileRepositoryStra()
        {
            _FileService = BaseFactory.GetFactory().GetService<IFileService>();
            _BackupService = BaseFactory.GetFactory().GetService<IBackupService>();
            _Logger = BaseFactory.GetFactory().GetService<ILogManager>().GetLogger<FileRepositoryStra>();
        }
    }
}
