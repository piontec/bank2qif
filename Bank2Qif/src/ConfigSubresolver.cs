using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.Core;
using Nini.Config;

namespace Bank2Qif
{
    public class ConfigSubresolver : ISubDependencyResolver
    {
        private readonly IConfigSource m_cfgSource;        
        private const string TRANSFORMER_SUFFIX = "Transformer";
        
        
        public ConfigSubresolver(IConfigSource src)
        {            
            m_cfgSource = src;
        }


        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, 
            ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetItemType == typeof(IConfig);
        }


        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, 
            ComponentModel model, DependencyModel dependency)
        {            
            var conf = m_cfgSource.Configs.Cast<IConfig>().
                Where(c => (c.Name + TRANSFORMER_SUFFIX).Equals (dependency.DependencyKey, StringComparison.CurrentCultureIgnoreCase)).
                SingleOrDefault();
            return conf != null ? conf : new IniConfig(dependency.DependencyKey, m_cfgSource);
        }
    }
}
