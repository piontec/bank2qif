using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Bank2Qif.Converters;

namespace Bank2Qif
{
    public class ConvertersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //var converters = from t in Assembly.GetExecutingAssembly().GetTypes()
            //                 where t.IsClass
            //                 let iface = t.GetInterface(typeof(IConverter).Name)
            //                 where iface != null
            //                 let convAttr = from a in Attribute.GetCustomAttributes(t)
            //                                where a is ConverterAttribute
            //                                select (ConverterAttribute)a
            //                 select new Tuple<Type, ConverterAttribute> ( t, convAttr.Single () );
            
            //container.Register(
            //    converters.Select (t => Component.For(typeof(IConverter)).ImplementedBy(t.Item1).Named(t.Item2.Bank)).
            //    Cast<IRegistration> ().ToArray ()                
            //    );

            container.Register(
                Classes.FromThisAssembly()
                .Where(t => typeof(IConverter).IsAssignableFrom(t) &&
                    Attribute.IsDefined(t, typeof(ConverterAttribute)))                
                .LifestyleSingleton()
                .WithService.Base()
                .Configure(c => c.Named(
                    ((ConverterAttribute)Attribute.GetCustomAttribute(
                    c.Implementation, typeof(ConverterAttribute))).Bank))
                );            
        }
    }
}
