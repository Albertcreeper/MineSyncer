using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories
{
    public interface ILocalRepository : IRepository
    {
        /// <summary>
        /// Process name of game to run synchronisation when process is closed
        /// </summary>
        string ProcessName { get; set; }

        /// <summary>
        /// Command line to start the game
        /// </summary>
        string StartCommandLine { get; set; }

        string RemoteRepositoryConfigPath { get; set; }

        IRemoteRepository RemoteRepository { get; set; }

        /// <summary>
        /// Lädt die Daten vom Remote Repository und überschreibt die lokalen Daten
        /// </summary>
        void PullFromRemote();

        /// <summary>
        /// Lädt die Daten vom aktuellen Repository hoch aufs Remote Repository und übeschreibt die Remote Daten
        /// </summary>
        void PushToRemote();

        /// <summary>
        /// Synchronize repository with remote repository
        /// </summary>
        void Synchronize();

        /// <summary>
        /// Get preview of synchronize (Nothing, PullFromRemote, PushToRemote)
        /// </summary>
        /// <returns>Preview of next synchronisation</returns>
        SyncPreview GetSynchronizePreview();

        /// <summary>
        /// Validate version and last change of repository to avoid invalid consistency
        /// </summary>
        void ValidateVersion();

        /// <summary>
        /// Check if current repository is NOT newer than remote repository
        /// </summary>
        /// <returns>True if current repository is newer than remote repository</returns>
        bool IsNotNewerThanRemote();

        /// <summary>
        /// Check if current repository is newer than remote repository
        /// </summary>
        /// <returns>True if current repository is newer than remote repository</returns>
        bool IsNewerThanRemote();

        /// <summary>
        /// Check if current repository is NOT older than remote repository
        /// </summary>
        /// <returns>True if current repository is older than remote repository</returns>
        bool IsNotOlderThanRemote();

        /// <summary>
        /// Check if current repository is older than remote repository
        /// </summary>
        /// <returns>True if current repository is older than remote repository</returns>
        bool IsOlderThanRemote();
    }
}
