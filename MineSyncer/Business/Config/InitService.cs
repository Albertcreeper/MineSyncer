using Business.Core;
using Business.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Business.Files
{
    public class InitService : IRepoConfigService
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetPrivateProfileString(
        string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString,
        uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool WritePrivateProfileString(
        string lpAppName, string lpKeyName, string lpString, string lpFileName);

        protected IFileService _FileService;

        public IRepository Repository { get; set; }

        public InitService(IFileService fileService)
        {
            _FileService = fileService;
        }
        public T Get<T>(string option)
        {
            return Get<T>(Repository.ConfigPath, Repository.Name, option);
        }

        public void Set(string option, object value)
        {
            Set(Repository.ConfigPath, Repository.Name, option, value);
        }

        protected T Get<T>(string filePath, string section, string key)
        {
            var valueBuilder = new StringBuilder(255);

            if (!_FileService.ExistsFile(filePath))
            {
                FileStream fileStream = File.Create(filePath);
                fileStream.Close();
            }

            GetPrivateProfileString(section, key, "", valueBuilder, 255, filePath);

            if (valueBuilder.Length == 0)
            {
                return default(T);
            }

            object value = valueBuilder.ToString();

            return value.ConvertTo<T>();
        }

        protected void Set(string filePath, string section, string key, object value)
        {
            if (!_FileService.ExistsFile(filePath))
            {
                FileStream fileStream = File.Create(filePath);
                fileStream.Close();
            }

            WritePrivateProfileString(section, key, value.ToString(), filePath);
        }
    }
}
