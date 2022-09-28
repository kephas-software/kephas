﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceCollectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceCollectionBuildContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Builder
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Principal;

    using Kephas.Injection.Configuration;
    using Kephas.Injection.Resources;
    using Kephas.Services;

    /// <summary>
    /// Contract interface for building <see cref="IAmbientServices"/>.
    /// </summary>
    public interface IAppServiceCollectionBuildContext : IContext
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
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The injector.
        /// </value>
        IServiceProvider IContext.ServiceProvider
            => throw new ServiceException(Strings.ServiceProviderIsBeingBuilt);

        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity? IContext.Identity
        {
            get => null;
            set { }
        }
    }
}