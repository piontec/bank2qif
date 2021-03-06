﻿using System;
using System.Collections.Generic;
using Bank2Qif.Converters;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;


namespace Bank2Qif
{
    public class ConvertersInstaller : IWindsorInstaller
    {
        public void Install (IWindsorContainer container, IConfigurationStore store)
        {
            container.Register (
                Classes.FromThisAssembly ()
                       .Where (t => typeof (IConverter).IsAssignableFrom (t) &&
                                    Attribute.IsDefined (t, typeof (ConverterAttribute)))
                       .LifestyleSingleton ()
                       .WithService.Select (new List<Type> {typeof (IConverter)})
                       .Configure (c => c.Named (
                           ((ConverterAttribute) Attribute.GetCustomAttribute (
                               c.Implementation, typeof (ConverterAttribute))).Bank))
                );
        }
    }
}