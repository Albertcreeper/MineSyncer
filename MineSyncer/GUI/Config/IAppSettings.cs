using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Config
{
    public interface IAppSettings
    {
        string RepositoriesPath { get; }

        string RepositoryName { get; }

        string LogConfigPath { get; }
    }
}
