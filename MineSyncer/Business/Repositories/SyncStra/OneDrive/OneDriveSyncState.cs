using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.SyncStra.OneDrive
{
    public enum OneDriveSyncState
    {
        Off, Unknown, Syncing, Synced, SharedSyncing, SharedSynced, Error
    }
}
