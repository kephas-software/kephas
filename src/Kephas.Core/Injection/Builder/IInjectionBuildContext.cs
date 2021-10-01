// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInjectionBuildContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Injection.Configuration;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for <see cref="IInjectorBuilder"/> contexts.
    /// </summary>
    public interface IInjectionBuildContext : IContext
    {
        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        IList<IAppServiceInfosProvider> AppServiceInfosProviders { get; }

        /// <summary>
        /// Gets the list of assemblies used in injection.
        /// </summary>
        IList<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the injection settings.
        /// </summary>
        InjectionSettings Settings { get; }
    }
}