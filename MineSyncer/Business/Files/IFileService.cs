using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Files
{
    public interface IFileService
    {
        bool ExistsFile(string path);

        bool ExistsFolder(string path);

        void CreateFolder(string path);

        void Copy(string sourcePath, string destinationPath, CopyOption options);

        List<String> GetFilesOfDirectory(string directoryPath);

        bool IsFolderEmpty(string directoryPath);

        DateTime GetLastChangeOfDirectory(string directoryPath);
    }
}
