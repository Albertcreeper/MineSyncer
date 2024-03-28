using Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files
{
    public interface IRepoConfigService
    {
        IRepository Repository { get; set; }

        T Get<T>(string option);

        void Set(string option, object value);
    }
}
