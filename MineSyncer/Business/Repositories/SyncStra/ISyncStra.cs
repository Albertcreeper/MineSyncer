using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra
{
    public interface ISyncStra
    {
        /// <summary>
        /// Lädt die Daten vom Remote Repository und überschreibt die lokalen Daten
        /// </summary>
        void PullFromRemote(ILocalRepository localRepository, IRemoteRepository remoteRepository);

        /// <summary>
        /// Lädt die Daten vom aktuellen Repository hoch aufs Remote Repository und übeschreibt die Remote Daten
        /// </summary>
        void PushToRemote(ILocalRepository localRepository, IRemoteRepository remoteRepository);
    }
}
