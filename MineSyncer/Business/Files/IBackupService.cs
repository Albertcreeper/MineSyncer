using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files
{
    public interface IBackupService
    {
        void CreateBackup(string source, string destination);
    }
}
