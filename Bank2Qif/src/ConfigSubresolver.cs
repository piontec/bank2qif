using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Nini.Config;


namespace Bank2Qif
{
    public class ConfigSubresolver : ISubDependencyResolver
    {
        private readonly IConfigSource m_cfgSource;
        private const string TRANSFORMER_SUFFIX = "Transformer";


        public ConfigSubresolver (IConfigSource src)
        {
            m_cfgSource = src;
        }


        public bool CanResolve (CreationContext context, ISubDependencyResolver contextHandlerResolver,
                                ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetItemType == typeof (IConfig);
        }


        public object Resolve (CreationContext context, ISubDependencyResolver contextHandlerResolver,
                               ComponentModel model, DependencyModel dependency)
        {
            var fullName = model.ComponentName.Name.Split ('.');
            var name = fullName [fullName.Length - 1];
            name = name.Substring (0, name.IndexOf (TRANSFORMER_SUFFIX));

            var conf = m_cfgSource.Configs.Cast<IConfig> ().FirstOrDefault (
                c => c.Name.Equals (name, StringComparison.CurrentCultureIgnoreCase));

            return conf ?? new IniConfig (name, m_cfgSource);
        }
    }
}