using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Files
{
    public enum CopyOption
    {
        Overwrite = 0x001,
        CopyOnlyChanges = 0x010
    }
}
