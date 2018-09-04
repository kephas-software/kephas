﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IHostConfigurator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
{
    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for configurating a host.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(RequiredFeatureAttribute) })]
    public interface IHostConfigurator
    {
        /// <summary>
        /// Configures the web host.
        /// </summary>
        /// <param name="configContext">Context for the configuration.</param>
        void Configure(IHostConfigurationContext configContext);
    }
}