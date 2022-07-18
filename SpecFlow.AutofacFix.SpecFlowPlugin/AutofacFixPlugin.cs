using System;
using Autofac;
using BoDi;
using SpecFlow.Autofac;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(SpecFlow.AutofacFix.SpecFlowPlugin.AutofacFixPlugin))]

namespace SpecFlow.AutofacFix.SpecFlowPlugin
{
    public class AutofacFixPlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterFactoryAs<IComponentContext>(() =>
                {
                    var containerBuilderFinder = args.ObjectContainer.Resolve<IContainerBuilderFinder>();

                    var featureScope = GetFeatureScope(args.ObjectContainer, containerBuilderFinder);

                    if (featureScope != null)
                    {
                        return featureScope.BeginLifetimeScope(nameof(ScenarioContext), builder =>
                        {
                            var configureScenarioContainer = containerBuilderFinder.GetConfigureScenarioContainer();
                            RegisterSpecflowDependencies(args.ObjectContainer, configureScenarioContainer(builder));
                        });
                    }

                    var createScenarioContainerBuilder = containerBuilderFinder.GetCreateScenarioContainerBuilder();
                    if (createScenarioContainerBuilder == null)
                    {
                        throw new Exception("Unable to find scenario dependencies! Mark a static method that has a ContainerBuilder parameter and returns void with [ScenarioDependencies]!");
                    }

                    var containerBuilder = createScenarioContainerBuilder(null);
                    RegisterSpecflowDependencies(args.ObjectContainer, containerBuilder);
                    args.ObjectContainer.RegisterFactoryAs(() => containerBuilder.Build());
                    return args.ObjectContainer.Resolve<IContainer>().BeginLifetimeScope(nameof(ScenarioContext));
                });
            };
        }

        private static ILifetimeScope GetFeatureScope(ObjectContainer objectContainer, IContainerBuilderFinder containerBuilderFinder)
        {
            var configureScenarioContainer = containerBuilderFinder.GetConfigureScenarioContainer();
            if (objectContainer.BaseContainer.IsRegistered<ILifetimeScope>() && configureScenarioContainer != null)
            {
                return objectContainer.BaseContainer.Resolve<ILifetimeScope>();
            }

            if (configureScenarioContainer != null)
            {
                var containerBuilder = new global::Autofac.ContainerBuilder();
                objectContainer.RegisterFactoryAs(() => containerBuilder.Build());
                return objectContainer.Resolve<IContainer>();
            }

            return null;
        }

        private void RegisterSpecflowDependencies(
            IObjectContainer objectContainer,
            global::Autofac.ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(ctx => objectContainer)
                .As<IObjectContainer>()
                .ExternallyOwned();

            containerBuilder.Register(
                ctx =>
                {
                    var specflowContainer = ctx.Resolve<IObjectContainer>();
                    var scenarioContext = specflowContainer.Resolve<ScenarioContext>();
                    return scenarioContext;
                }).As<ScenarioContext>();
            containerBuilder.Register(
                ctx =>
                {
                    var specflowContainer = ctx.Resolve<IObjectContainer>();
                    var scenarioContext = specflowContainer.Resolve<FeatureContext>();
                    return scenarioContext;
                }).As<FeatureContext>();
            containerBuilder.Register(
                ctx =>
                {
                    var specflowContainer = ctx.Resolve<IObjectContainer>();
                    var scenarioContext = specflowContainer.Resolve<TestThreadContext>();
                    return scenarioContext;
                }).As<TestThreadContext>();
        }
    }
}
