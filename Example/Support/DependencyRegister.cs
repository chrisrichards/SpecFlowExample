using System;
using System.Linq;
using Autofac;
using SpecFlow.Autofac;

namespace Example.Support
{
    public static class DependecyRegister
    {
        [ScenarioDependencies]
        public static ContainerBuilder CreateContainerBuilder()
        {
            var builder = new ContainerBuilder();

            // We're registering all test classes from assembly with [Binding] annotation
            builder
                .RegisterTypes(typeof(DependecyRegister).Assembly.GetTypes()
                                                        .Where(t => Attribute.IsDefined(t, typeof(BindingAttribute)))
                                                        .ToArray())
                .SingleInstance();
            return builder;
        }
    }
}