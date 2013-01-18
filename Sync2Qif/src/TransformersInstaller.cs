using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Transformers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Bank2Qif
{
    public class TransformersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly()
                .Where (t => typeof(ITransformer).IsAssignableFrom (t) && 
                    Attribute.IsDefined (t, typeof (TransformerAttribute)))                                
                .LifestyleSingleton ()                
                .WithService.Select(new List<Type> { typeof(ITransformer) })
                );
        }
    }
}
