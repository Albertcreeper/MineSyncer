using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace GUI.Config
{
    public class AppSettings : IAppSettings
    {
        public string RepositoriesPath { get; set; }

        public string RepositoryName { get; set; }

        public string LogConfigPath { get; set; }

        public AppSettings()
        {

        }

        static AppSettings()
        {

        }
    }
}
