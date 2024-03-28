using Business.Repositories.SyncStra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories
{
    public interface IRemoteRepository : IRepository
    {
        /// <summary>
        /// Type of remote repository (e.g. OneDrive, File, ...)
        /// </summary>
        RemoteRepositoryType RepositoryType { get; }

        /// <summary>
        /// Get sync strategy of current remote repository
        /// </summary>
        /// <returns></returns>
        ISyncStra GetSyncStra();
    }
}
