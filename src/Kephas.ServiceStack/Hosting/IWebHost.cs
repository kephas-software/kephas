// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebHost.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWebHost interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
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