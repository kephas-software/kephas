// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebHost.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IWebHost interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting
{
    using System;

    using Funq;

    using global::ServiceStack;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for the web host.
    /// </summary>
    [SharedAppServiceContract]
    public interface IWebHost : IAppHost, IFunqlet, IHasContainer, IDisposable, IAsyncInitializable, IAsyncFinalizable
    {
    }
}