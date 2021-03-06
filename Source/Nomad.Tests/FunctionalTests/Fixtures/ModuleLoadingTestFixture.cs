using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;
using Nomad.Communication.EventAggregation;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using Nomad.Tests.FunctionalTests.Modules;
using Nomad.Utils.ManifestCreator;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Fixtures
{
    public abstract class ModuleLoadingTestFixture
    {
        protected const string IssuerXmlPath = "KEY.xml";
        protected const string IssuerName = "KEY_issuer";
        protected WindsorContainer Container;
        protected ModuleManager Manager;
        private InjectableModulesRegistry _registry;


        [TestFixtureSetUp]
        public virtual void SetUpFixture()
        {
            if (File.Exists(IssuerXmlPath))
            {
                File.Delete(IssuerXmlPath);
            }
            KeysGeneratorProgram.Main(new[] {IssuerXmlPath});
        }


        [TestFixtureTearDown]
        public virtual void CleanUpFixture()
        {
            if (File.Exists(IssuerXmlPath))
            {
                File.Delete(IssuerXmlPath);
            }
        }


        protected void CopyModuleIntoDirectory(string from, string to)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(to));
            File.Copy(from, to, true);
        }


        protected void SignModule(string name, string path)
        {
            var builder = new ManifestBuilder(IssuerName, IssuerXmlPath, name, path);
            builder.CreateAndPublish();
        }


        [SetUp]
        public void clear_registries_and_create_module_manager()
        {
            _registry = new InjectableModulesRegistry();
            LoadedModulesRegistry.Clear();

            Container = new WindsorContainer();
            Container.Register(
                Component.For<IInjectableModulesRegistry>().Instance(_registry)
                );

            var dependencyCheckerMock = new Mock<IDependencyChecker>(MockBehavior.Loose);
            dependencyCheckerMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>()))
                .Returns<IEnumerable<ModuleInfo>>(e => e);

            Manager = new ModuleManager(new ModuleLoader(Container, new NullGuiThreadProvider()),
                                        new CompositeModuleFilter(),
                                        dependencyCheckerMock.Object);
        }


        protected void InvokeUnloadMethod()
        {
            Manager.InvokeUnloadCallback();
        }


        protected void AssertInvokeUnloadMethodsWasInvoked(params string[] expectedModuleNames)
        {
            string[] unloadedModuleNames = LoadedModulesRegistry.GetUnRegisteredModules()
                .Select(type => type.Name)
                .ToArray();

            Assert.That(unloadedModuleNames, Is.EqualTo(expectedModuleNames));
        }


        protected void LoadModulesFromDirectory(IModuleDiscovery moduleDiscovery)
        {
            Manager.LoadModules(moduleDiscovery);
        }


        protected void AssertModulesLoadedAreEqualTo(params string[] expectedModuleNames)
        {
            string[] loadedModulesNames = LoadedModulesRegistry.GetRegisteredModules()
                .Concat(_registry.GetRegisteredModules())
                .Select(type => type.Name)
                .ToArray();

            Assert.That(loadedModulesNames, Is.EqualTo(expectedModuleNames));
        }
    }
}