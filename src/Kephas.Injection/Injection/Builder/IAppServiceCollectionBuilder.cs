// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceCollectionBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceCollectionBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Injection.Configuration;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for <see cref="IAmbientServices"/> builders.
    /// </summary>
    public interface IAppServiceCollectionBuilder : IDynamic, ILoggable
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        ICollection<IAppServiceInfosProvider> AppServiceInfosProviders { get; }

        /// <summary>
        /// Gets the list of assemblies used in injection.
        /// </summary>
        ICollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets the injection settings.
        /// </summary>
        InjectionSettings Settings { get; }

        /// <summary>
        /// Adds the application services from the <see cref="IAppServiceInfosProvider"/>s identified in the assemblies.
        /// </summary>
        /// <returns>The provided ambient services.</returns>
        IAmbientServices Build();
    }
}