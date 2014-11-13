// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsStorePlatformManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Platform manager for Windows Store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Platform manager for Windows Store.
    /// </summary>
    public class WindowsStorePlatformManager : IPlatformManager
    {
        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public async Task<IEnumerable<Assembly>> GetAppAssembliesAsync()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            var assemblies = new List<Assembly>();
            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    var name = new AssemblyName { Name = file.Name };
                    var asm = Assembly.Load(name);
                    assemblies.Add(asm);
                }
            }

            return assemblies;
        }
    }
}