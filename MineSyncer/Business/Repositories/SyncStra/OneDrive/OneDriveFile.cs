using Business.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra.OneDrive
{
    public class OneDriveFile
    {
        public string Path { get; protected set; }

        public OverlayIconInfo IconInfo { get; protected set; }

        public FileInfo FileInfo { get; protected set; }

        public OneDriveFile(string filePath)
        {
            Path = filePath;
            FileInfo = new FileInfo(Path);
            IconInfo = new OverlayIconInfo(FileInfo);
        }

        public void Refresh()
        {
            FileInfo = new FileInfo(Path);
            IconInfo = new OverlayIconInfo(FileInfo);
        }
    }
}
