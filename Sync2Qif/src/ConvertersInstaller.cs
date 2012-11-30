using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Sync2Qif.Converters;
using System.Reflection;

namespace Sync2Qif
{
    public class ConvertersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var converters = from t in Assembly.GetExecutingAssembly().GetTypes()
                             where t.IsClass
                             let iface = t.GetInterface(typeof(IConverter).Name)
                             where iface != null
                             let convAttr = from a in Attribute.GetCustomAttributes(t)
                                            where a is ConverterAttribute
                                            select (ConverterAttribute)a
                             select new Tuple<Type, ConverterAttribute> ( t, convAttr.Single () );
            foreach (var tuple in converters)
                container.Register(
                    Component.For(typeof (IConverter)).ImplementedBy(tuple.Item1).Named(tuple.Item2.Bank)
                    );            
        }
    }
}
