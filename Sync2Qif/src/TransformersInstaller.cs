using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Transformers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Sync2Qif
{
    public class TransformersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly()
                .BasedOn<ITransformer>()
                .Configure (c => c.Named (c.Attribute ("ky").ToString ()))
                .LifestyleSingleton ()
                .WithService.Base()
                );
        }
    }
}
