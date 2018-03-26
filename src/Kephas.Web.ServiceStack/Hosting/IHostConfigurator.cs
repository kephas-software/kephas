// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IHostConfigurator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting
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