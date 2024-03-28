using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Business.Files
{
    public class FileService : IFileService
    {
        protected static ILogger<FileService> _Logger;

        public FileService(ILogManager logManager)
        {
            _Logger = logManager.GetLogger<FileService>();
        }

        public void Copy(string source, string destination, CopyOption options)
        {
            _Logger.LogDebug($"Copy files from source <{source}> to destination <{destination}> with options: {options}");

            if (options == CopyOption.CopyOnlyChanges)
            {
                CopyOnlyChanges(source, destination, options);
                return;
            }

            DirectoryInfo sourceInfo = new DirectoryInfo(source);
            DirectoryInfo destinationInfo = new DirectoryInfo(destination);

            Copy(sourceInfo, destinationInfo);
        }

        public void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool ExistsFile(string path)
        {
            return File.Exists(path);
        }

        public bool ExistsFolder(string path)
        {
            return Directory.Exists(path);
        }

        public List<String> GetFilesOfDirectory(string directoryPath)
        {
            List<String> files = new List<String>();

            foreach (string f in Directory.GetFiles(directoryPath))
            {
                _Logger.LogDebug($"Found file <{f}>");
                files.Add(f);
            }
            foreach (string d in Directory.GetDirectories(directoryPath))
            {
                _Logger.LogDebug($"Found directory <{d}> -> get all files from directory <{d}>");
                files.AddRange(GetFilesOfDirectory(d));
            }

            return files;
        }

        public bool IsFolderEmpty(string directoryPath)
        {
            return !GetFilesOfDirectory(directoryPath).Any();
        }

        protected List<String> GetChangedFiles(string sourcePath, List<string> sourceFiles,  string destPath, List<string> destFiles)
        {
            var changedFiles = new List<String>();

            foreach (String file in sourceFiles)
            {
                if(destFiles.Contains(file))
                {
                    DateTime sourceChanged = File.GetLastWriteTimeUtc(sourcePath + file);
                    DateTime destChanged = File.GetLastWriteTimeUtc(destPath + file);

                    if(sourceChanged != destChanged)
                    {
                        changedFiles.Add(file);
                    }
                }
            }

            return changedFiles;
        }

        protected List<String> GetDeletedFiles(List<string> sourceFiles, List<string> destFiles)
        {
            var removedFiles = new List<String>();

            foreach (var file in destFiles)
            {
                if (!sourceFiles.Contains(file))
                {
                    removedFiles.Add(file);
                }
            }

            return removedFiles;
        }

        protected List<String> GetAddedFiles(List<string> sourceFiles, List<string> destFiles)
        {
            var addedFiles = new List<String>();

            foreach(var file in sourceFiles)
            {
                if(!destFiles.Contains(file))
                {
                    addedFiles.Add(file);
                }
            }

            return addedFiles;
        }

        protected void ChangeValues<T>(List<T> list, Func<T, T> changeAction)
        {
            if(!list.Any())
            {
                return;
            }

            for(int i = 0; i < list.Count; i++)
            {
                list[i] = changeAction(list[i]);
            }
        }

        protected void Copy(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                Copy(dir, destination.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name));
            }
        }

        protected void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            string directoryName = destFileName.Substring(0, destFileName.LastIndexOf("\\"));

            if(!ExistsFolder(directoryName))
            {
                CreateFolder(directoryName);
            }

            File.Copy(sourceFileName, destFileName, overwrite);
        }

        protected void CopyOnlyChanges(string source, string destination, CopyOption options)
        {
            //1. get files from directories
            _Logger.LogDebug($"Get all files from source and destination");
            var sourceFiles = GetFilesOfDirectory(source);
            var destFiles = GetFilesOfDirectory(destination);

            //2. remove unnecessary path
            ChangeValues(sourceFiles, (file) => file.Replace(source, string.Empty));
            ChangeValues(destFiles, (file) => file.Replace(destination, string.Empty));

            //3. copy new files
            _Logger.LogDebug($"Copy new files to destination <{destination}>");
            var files = GetAddedFiles(sourceFiles, destFiles);
            foreach (String file in files)
            {
                _Logger.LogDebug($"Copy new file from source to destination: <{source + file}> => <{destination + file}>");
                CopyFile(source + file, destination + file, true);
            }

            //4. delete deleted files
            _Logger.LogDebug($"Remove all deleted files on source from destination <{destination}>");
            files = GetDeletedFiles(sourceFiles, destFiles);
            foreach (String file in files)
            {
                _Logger.LogDebug($"Delete removed file from destination: <{destination + file}>");
                File.Delete(destination + file);
            }

            //5. copy changed files
            files = GetChangedFiles(source, sourceFiles, destination, destFiles);
            foreach (String file in files)
            {
                _Logger.LogDebug($"Copy changed file from source to destination: <{source + file}> => <{destination + file}>");
                CopyFile(source + file, destination + file, true);
            }

            _Logger.LogDebug($"Finished coping files from source <{source}> to destination <{destination}>");
        }

        public DateTime GetLastChangeOfDirectory(string directoryPath)
        {
            List<string> files = GetFilesOfDirectory(directoryPath);
            string lastfile = files.OrderByDescending(f => new FileInfo(f).LastWriteTime).FirstOrDefault();

            if(lastfile is null)
            {
                return DateTime.MinValue;
            }

            return new FileInfo(lastfile).LastWriteTime;
        }
    }
}
