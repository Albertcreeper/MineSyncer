using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Repositories
{
    /// <summary>
    /// Lokales Repository um Daten mit einem Remote Repository zu synchronisieren
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Name vom Repository
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Path of configuration (e.g. ini file)
        /// </summary>
        string ConfigPath { get; set; }

        /// <summary>
        /// Pfad vom Repository
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Sicherungspfad vom Repository, wo Backups erstellt werden sollen, wenn das Repository synchronisiert / gesichert wird.
        /// </summary>
        string BackupPath { get; set; }

        /// <summary>
        /// Angabe, ob Sicherungen erstellt werden sollen, wenn das Repository synchronisiert wird.
        /// </summary>
        bool CreateBackup { get; set; }

        /// <summary>
        /// Current version
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Last change of repository (last savegame modification)
        /// </summary>
        DateTime LastChange { get; }

        /// <summary>
        /// Variables to resolve paths, ...
        /// </summary>
        Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Refresh repository from init file
        /// </summary>
        void Refresh();

        /// <summary>
        /// Operations after refresh (e.g. resolve paths)
        /// </summary>
        void AfterRefresh();

        /// <summary>
        /// Write changes to init file
        /// </summary>
        void Save();

        /// <summary>
        /// Check if repository exists
        /// </summary>
        /// <returns>True if repository exists; otherwise false</returns>
        bool Exists();

        /// <summary>
        /// Check if repository is empty or not
        /// </summary>
        /// <returns>True if repository is empty and contains no files; otherwise false</returns>
        bool IsEmpty();

        /// <summary>
        /// Create new repository
        /// </summary>
        void Create();

        /// <summary>
        /// Create backup of repository
        /// </summary>
        void CreateBackupOfRepository();

        /// <summary>
        /// Check if current repository is newer than other repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>True if current repository is newer than handed over repository</returns>
        bool IsNewerThan(IRepository repository);

        /// <summary>
        /// Check if current repository is NOT newer than other repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>True if current repository is NOT newer than handed over repository</returns>
        bool IsNotNewerThan(IRepository repository);

        /// <summary>
        /// Check if current repository is NOT older than other repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>True if current repository is older than handed over repository</returns>
        bool IsNotOlderThan(IRepository repository);

        /// <summary>
        /// Check if current repository is older than other repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>True if current repository is older than handed over repository</returns>
        bool IsOlderThan(IRepository repository);
    }
}
