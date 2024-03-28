using Business.Core;
using Business.Files;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra.OneDrive
{
    public class OneDriveSyncStra : ISyncStra
    {
        protected IFileService _FileService;
        protected static ILogger<OneDriveSyncStra> _Logger;

        public static readonly string Name = "OneDrive";

        public OneDriveSyncStra()
        {
            _FileService = BaseFactory.GetFactory().GetService<IFileService>();
            _Logger = BaseFactory.GetFactory().GetService<ILogManager>().GetLogger<OneDriveSyncStra>();
        }

        public void PullFromRemote(ILocalRepository localRepository, IRemoteRepository remoteRepository)
        {
            throw new NotImplementedException();
        }

        public void PushToRemote(ILocalRepository localRepository, IRemoteRepository remoteRepository)
        {
            Task.Run(() => PushToRemoteAsync(localRepository, remoteRepository));
        }

        protected async Task PushToRemoteAsync(ILocalRepository localRepository, IRemoteRepository remoteRepository)
        {
            var files = _FileService.GetFilesOfDirectory(remoteRepository.Path).Select(f => new OneDriveFile(f)).ToList();

            await Task.Run(() => { _FileService.Copy(localRepository.Path, remoteRepository.Path, CopyOption.CopyOnlyChanges);  });
            await Task.Delay(1000);

            while (files.Any())
            {
                foreach(var file in files)
                {
                    file.Refresh();

                    if(GetSyncState(file.IconInfo) == OneDriveSyncState.Synced)
                    {
                        files.Remove(file);
                    }

                    await Task.Delay(1000);
                }
            }
        }

        protected OneDriveSyncState GetSyncStateOfFile(string filePath)
        {
            return GetSyncState(new OverlayIconInfo(new FileInfo(filePath)));
        }

        protected OneDriveSyncState GetSyncStateOfFolder(string folderPath)
        {
            return GetSyncState(new OverlayIconInfo(new DirectoryInfo(folderPath)));
        }

        protected OneDriveSyncState GetSyncState(OverlayIconInfo iconInfo)
        {
            if (iconInfo != null)
            {
                if (iconInfo.IconOverlayGuid == Guid.Empty) return OneDriveSyncState.Off;
                else if (iconInfo.IconOverlayGuid == new Guid("{BBACC218-34EA-4666-9D7A-C78F2274A524}")) return OneDriveSyncState.Error;
                else if (iconInfo.IconOverlayGuid == new Guid("{F241C880-6982-4CE5-8CF7-7085BA96DA5A}")) return OneDriveSyncState.Synced;
                else if (iconInfo.IconOverlayGuid == new Guid("{A0396A93-DC06-4AEF-BEE9-95FFCCAEF20E}")) return OneDriveSyncState.Syncing;
                else if (iconInfo.IconOverlayGuid == new Guid("{5AB7172C-9C11-405C-8DD5-AF20F3606282}")) return OneDriveSyncState.SharedSynced;
                else if (iconInfo.IconOverlayGuid == new Guid("{A78ED123-AB77-406B-9962-2A5D9D2F7F30}")) return OneDriveSyncState.SharedSyncing;
                else return OneDriveSyncState.Unknown;
            }
            else return OneDriveSyncState.Unknown;
        }
    }
}
