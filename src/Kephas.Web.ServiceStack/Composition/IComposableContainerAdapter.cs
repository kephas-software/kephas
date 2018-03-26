// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IComposableContainerAdapter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IComposableContainerAdapter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Composition
{
    using Kephas.Services;

    using global::ServiceStack.Configuration;

    /// <summary>
    /// Shared container adapter application service contract.
    /// </summary>
    [SharedAppServiceContract]
    public interface IComposableContainerAdapter : IContainerAdapter
    {
    }
}