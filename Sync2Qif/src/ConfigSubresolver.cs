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
        private readonly IniConfigSource m_iniSource;
        private const string INI_NAME = @"etc/config.ini";
        private const string TRANSFORMER_SUFFIX = "Transformer";

        public ConfigSubresolver()
        {
            m_iniSource = new IniConfigSource(INI_NAME);
        }


        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, 
            ComponentModel model, DependencyModel dependency)
        {            
            return context.RequestedType == typeof (IConfig);
            //dependency.TargetItemType
        }


        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, 
            ComponentModel model, DependencyModel dependency)
        {            
            var conf = m_iniSource.Configs.Cast<IConfig>().
                Where(c => (c.Name + TRANSFORMER_SUFFIX).Equals (dependency.DependencyKey, StringComparison.CurrentCultureIgnoreCase)).
                SingleOrDefault();
            return conf != null ? conf : new IniConfig(dependency.DependencyKey, m_iniSource);
        }
    }
}
