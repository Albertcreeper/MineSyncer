using Business.Core;
using Business.Files;
using Business.Process;
using Business.Repositories;
using Business.Repositories.SyncStra;
using Business.Repositories.SyncStra.File;
using Business.Repositories.SyncStra.OneDrive;
using GUI.Config;
using Logger;
using Logger.Log4Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        BaseFactory Factory { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "GUI.dll.config");

            Factory = BaseFactory.GetFactory();
            Factory.AddXmlConfiguration(configPath);
            Factory.BindXmlConfiguration<AppSettings>("appSettings");

            Factory.AddSingletonService<ILogManager, Log4NetManager>();
            Factory.AddSingletonService<IProcessManager, ProcessManager>();
            Factory.AddTransientService<IFileService, FileService>();
            Factory.AddTransientService<IRepoConfigService, InitService>();
            Factory.AddTransientService<IBackupService, ZipBackupService>();

            Factory.AddTransientService<OneDriveSyncStra, OneDriveSyncStra>();
            Factory.AddTransientService<OneDriveRepoStra, OneDriveRepoStra>();
            Factory.AddTransientService<FileSyncStra, FileSyncStra>();
            Factory.AddTransientService<FileRepositoryStra, FileRepositoryStra>();

            Factory.BuildServices();

            Factory.Put("RepositorySyncStra." + OneDriveSyncStra.Name, Factory.GetService<OneDriveSyncStra>());
            Factory.Put("RepositoryStra." + OneDriveRepoStra.Name, Factory.GetService<OneDriveRepoStra>());

            Factory.Put("RepositorySyncStra." + FileSyncStra.Name, Factory.GetService<FileSyncStra>());
            Factory.Put("RepositoryStra." + FileRepositoryStra.Name, Factory.GetService<FileRepositoryStra>());
            

            ConfigureLogging();
        }

        protected void ConfigureLogging()
        {
            IAppSettings appConfig = BaseFactory.GetFactory().GetConfiguration<AppSettings>();
            ILogManager logManager = BaseFactory.GetFactory().GetService<ILogManager>();

            ((Log4NetManager)logManager).ConfigureLogging(appConfig.LogConfigPath);
        }
    }
}
