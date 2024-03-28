using Business.Core;
using Business.Core.Exceptions;
using Business.Files;
using Business.Repositories.Extensions;
using Business.Repositories.SyncStra;
using Business.Repositories.SyncStra.File;
using Logger;
using Logger.Log4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Business.Repositories
{
    public class Repository : IRepository
    {
        protected IFileService _FileService;
        protected IRepoConfigService _ConfigService;
        protected IBackupService _BackupService;
        protected static ILogger<Repository> _Logger;

        protected IRepositoryStra _Stra;

        public Repository(string name, string configPath)
        {
            _FileService = BaseFactory.GetFactory().GetService<IFileService>();
            _ConfigService = BaseFactory.GetFactory().GetService<IRepoConfigService>();
            _BackupService = BaseFactory.GetFactory().GetService<IBackupService>();
            _Logger = BaseFactory.GetFactory().GetService<ILogManager>().GetLogger<Repository>();

            Name = name;
            ConfigPath = configPath;
            Variables = new Dictionary<string, string>();
            _ConfigService.Repository = this;

            Refresh();
        }

        public string Name { get; set; }

        public string ConfigPath { get; set; }

        public string Path { get; set; }

        public string BackupPath { get; set; }

        public bool CreateBackup { get; set; } = true;

        public int Version { get; set; }
        public DateTime LastChange { get => _Stra.GetLastChangeOfRepository(this); }

        public Dictionary<string, string> Variables { get; set; }

        public void CreateBackupOfRepository()
        {
            _Stra.CreateBackupOfRepository(this);
        }

        public bool Exists()
        {
            return _Stra.Exists(this);
        }

        public bool IsEmpty()
        {
            return _Stra.IsEmpty(this);
        }

        public void Create()
        {
            _Stra.Create(this);
        }

        public virtual void Refresh()
        {
            Name = _ConfigService.Get<string>("Name");
            Path = _ConfigService.Get<string>("Path");
            BackupPath = _ConfigService.Get<string>("BackupPath");
            CreateBackup = _ConfigService.Get<bool>("CreateBackup");
            Version = _ConfigService.Get<int>("Version");

            _Stra = BaseFactory.GetFactory().GetStra(FileRepositoryStra.Name);

            AfterRefresh();
        }

        public virtual void AfterRefresh()
        {

        }

        public virtual void Save()
        {
            _Logger.LogDebug("Save data");

            _ConfigService.Set("Name", Name);
            _ConfigService.Set("Path", Path);
            _ConfigService.Set("BackupPath", BackupPath);
            _ConfigService.Set("CreateBackup", CreateBackup);
            _ConfigService.Set("Version", Version);
        }

        public override string ToString()
        {
            return $"Name={Name}|ConfigPath={ConfigPath}|Path={Path}|" +
                $"CreateBackup={CreateBackup}|BackupPath={BackupPath}|Version={Version}|LastChange={LastChange}|Stra={_Stra.ToString()}";
        }

        public bool IsNotNewerThan(IRepository repository)
        {
            return !IsNewerThan(repository);
        }

        public bool IsNewerThan(IRepository repository)
        {
            return Version > repository.Version ||
                LastChange > repository.LastChange;
        }

        public bool IsNotOlderThan(IRepository repository)
        {
            return !IsOlderThan(repository);
        }

        public bool IsOlderThan(IRepository repository)
        {
            return Version < repository.Version ||
                LastChange < repository.LastChange;
        }

        public static Dictionary<string, string> ConvertToVariables(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new Dictionary<string, string>();
            }

            try
            {
                return value.Split(";").ToDictionary(keySelector: key => key.Split("=")[0], elementSelector: m => m.Split("=")[1]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not read variables", ex);
            }
        }

        public static string ConvertToString(Dictionary<string, string> dictionary)
        {
            if (dictionary is null || !dictionary.Any())
            {
                return string.Empty;
            }

            string output = "";
            foreach (KeyValuePair<string, string> keyValue in dictionary)
            {
                output += $"{keyValue.Key}={keyValue.Value};";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }
    }
}
