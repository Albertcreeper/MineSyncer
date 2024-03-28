using Business.Core;
using Business.Repositories.SyncStra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.Extensions
{
    public static class BasicFactoryExtensions
    {
        public static IRepositoryStra GetStra(this BaseFactory factory, string name)
        {
            var obj = factory.Get("RepositoryStra." + name);

            if(obj is IRepositoryStra)
            {
                return (IRepositoryStra)obj;
            }

            throw new ApplicationException($"Object <{name}> is not a repository strategy!");
        }

        public static ISyncStra GetSyncStra(this BaseFactory factory, string name)
        {
            var obj = factory.Get("RepositorySyncStra." + name);

            if (obj is ISyncStra)
            {
                return (ISyncStra)obj;
            }

            throw new ApplicationException($"Object <{name}> is not a repository sync strategy!");
        }
    }
}
