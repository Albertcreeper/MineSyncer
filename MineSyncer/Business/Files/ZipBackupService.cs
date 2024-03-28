using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files
{
    public class ZipBackupService : IBackupService
    {
        private static ILogger<ZipBackupService> _Logger;

        protected IFileService _FileService;

        public ZipBackupService(ILogManager logManager, IFileService fileService)
        {
            _Logger = logManager.GetLogger<ZipBackupService>();
            _FileService = fileService;
        }

        public void CreateBackup(string source, string destination)
        {
            _Logger.LogInfo($"Create zip backup: SOURCE: {source} | DESTINATION: {destination}");

            _Logger.LogDebug($"Capture all files from source");
            List<string> files = _FileService.GetFilesOfDirectory(source);
            ChangeValues(files, (file) => file.Replace(source + "\\", string.Empty));

            destination += ".zip";
            _Logger.LogDebug($"Create new zip archive: ZIP: {destination}");
            using (ZipArchive archive = ZipFile.Open(destination, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    archive.CreateEntryFromFile(source + "\\" + file, file);
                }
            }

            _Logger.LogInfo("Successfully created zip file");
        }

        protected string GetNextBackupName()
        {
            return DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
        }

        protected void ChangeValues<T>(List<T> list, Func<T, T> changeAction)
        {
            if (!list.Any())
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = changeAction(list[i]);
            }
        }
    }
}
