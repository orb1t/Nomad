﻿using System;
using System.Threading;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Utils.ManifestCreator;

namespace ThinWpfHost
{
    /// <summary>
    /// Sample WpfApplication loader 
    /// </summary>
    /// <remarks>
    /// This tutorial shows how to load & unload & load again wpf application
    /// </remarks>
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            // signing the assemblies and creating the manifest using manifestBuilder api
            GenerateManifestUsingApi("WpfApplicationModule.exe", @".\Modules\WpfApplication");
            GenerateManifestUsingApi("WpfButtonModule.dll", @".\Modules\WpfButton");

            // using default configuration
            var kernel = new NomadKernel();

            // loading modules using single module discovery pattern
            var discovery = new CompositeModuleDiscovery(
                new SingleModuleDiscovery(@".\Modules\WpfApplication\WpfApplicationModule.exe"),
                new SingleModuleDiscovery(@".\Modules\WpfButton\WpfButtonModule.dll")
                );

            kernel.LoadModules(discovery);

            //wait for input
            //Console.ReadLine();

            //simulate reloading :]
            /*Thread.Sleep(15000);

            kernel.UnloadModules();

            //TODO: Here we might to try to substitute dll with new version to see if it was really unloaded

            Thread.Sleep(2000);

            kernel.LoadModules(discovery);

            Thread.Sleep(5000);
            */
            //TODO: Here we should wait for event "onclose", because we are going to loose nomad ;)
        }


        private static void GenerateManifestUsingApi(string assemblyName, string path)
        {
            var builder = new ManifestBuilder(@"TUTORIAL_ISSUER",
                                              @"..\..\KEY_FILE.xml",
                                              assemblyName,
                                              path);
            builder.CreateAndPublish();
        }
    }
}