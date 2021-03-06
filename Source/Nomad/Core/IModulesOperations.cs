﻿using System.Collections.Generic;
using Nomad.Modules;
using Nomad.Modules.Discovery;

namespace Nomad.Core
{
    ///<summary>
    /// Responsible for loading and unloading modules
    ///</summary>
    public interface IModulesOperations
    {
        /// <summary>
        ///     Unloads all modules and releases their files handlers
        /// </summary>
        void UnloadModules();


        /// <summary>
        /// Loads all modules
        /// </summary>
        void LoadModules(IModuleDiscovery moduleDiscovery);


        /// <summary>
        ///     Gets all the loaded modules.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ModuleInfo> GetLoadedModules();
    }
}