using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{
    public class BaseFactory
    {
        protected static BaseFactory _Instance;

        protected IServiceCollection _Services = new ServiceCollection();
        protected ServiceProvider _ServiceProvider;
        protected ConfigurationManager _ConfigManager = new();
        protected Dictionary<Type, object> _Configurations = new ();
        protected Dictionary<string, object> _Objects = new Dictionary<string, object>();

        protected BaseFactory()
        {
            _ConfigManager = new();
        }

        public void AddXmlConfiguration(string filePath)
        {
            var xmlSource = new XmlConfigurationSource() { Path = filePath };
            xmlSource.ResolveFileProvider();
            var configRoot = new ConfigurationRoot(new List<IConfigurationProvider>() { new XmlConfigurationProvider(xmlSource) });

            _ConfigManager.AddConfiguration(configRoot);
        }

        public void BindXmlConfiguration<TImplementation>(string sectionName) where TImplementation : new()
        {
            TImplementation configuration = new TImplementation();
            IConfigurationSection configSection = _ConfigManager.GetSection(sectionName);
            configSection.Bind(configuration);

            _Configurations.Add(configuration.GetType(), configuration);
        }

        public void AddScopedService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _Services.AddScoped<TService, TImplementation>();
        }

        public void AddSingletonService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _Services.AddSingleton<TService, TImplementation>();
        }

        public void AddTransientService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _Services.AddTransient<TService, TImplementation>();
        }

        public TImplementation GetConfiguration<TImplementation>()
        {
            return (TImplementation)_Configurations[typeof(TImplementation)];
        }

        public TService GetService<TService>()
        {
            return _ServiceProvider.GetService<TService>();
        }

        public void BuildServices()
        {
            _ServiceProvider = _Services.BuildServiceProvider();
        }

        public void Put(string name, object @object)
        {
            _Objects.Add(name, @object);
        }

        public object Get(string name)
        {
            return _Objects[name];
        }

        public static BaseFactory GetFactory()
        {
            if(_Instance is null)
            {
                _Instance = new BaseFactory();
            }

            return _Instance;
        }
    }
}
