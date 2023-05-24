// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadContextAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks;

using Kephas.Runtime;

/// <summary>
/// Assembly initializer for the thread context.
/// </summary>
public class ThreadContextAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        new ThreadContextBuilder()
            .WithStoreAction(ThreadHelper.StoreThreadCulture)
            .WithRestoreAction(ThreadHelper.RestoreThreadCulture);
    }
}