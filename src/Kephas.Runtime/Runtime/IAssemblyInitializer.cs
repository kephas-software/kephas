// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime;

using System.Reflection;

/// <summary>
/// Interface for components doing assembly initialization.
/// Initializers implementing this interface must have a parameterless constructor.
/// </summary>
public interface IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    void Initialize();

#if NETSTANDARD2_1

    private static readonly object SyncObject = new object();
    private static bool initialized = false;

    /// <summary>
    /// Initializes the assemblies by identifying the <see cref="IAssemblyInitializer"/>s and invoking their <see cref="Initialize"/> method.
    /// </summary>
    /// <remarks>
    /// This method is not required for .NET 6.0 and newer, because the initializers are automatically called upon assembly load.
    /// Make sure to reference Kephas.Analyzers to get this automatic behavior.
    /// </remarks>
    public static void InitializeAssemblies()
    {
        if (initialized)
        {
            return;
        }

        lock (SyncObject)
        {
            if (initialized)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => InitializeAssembly(args.LoadedAssembly);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                InitializeAssembly(assembly);
            }

            initialized = true;
        }
    }

    private static void InitializeAssembly(Assembly assembly)
    {
        try
        {
            foreach (var type in assembly.ExportedTypes)
            {
                if (!type.IsClass || type.IsAbstract || !typeof(IAssemblyInitializer).IsAssignableFrom(type))
                {
                    continue;
                }

                try
                {
                    if (Activator.CreateInstance(type) is IAssemblyInitializer initializer)
                    {
                        initializer.Initialize();
                    }
                }
                catch (Exception)
                {
                    // do not log anything, during initialization we don't have a proper way of notification.
                }
            }
        }
        catch (Exception)
        {
            // do not log anything, during initialization we don't have a proper way of notification.
        }
    }
#endif
}
