// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOwinAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOwinAppContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Owin.Application
{
    using global::Owin;

    using Kephas.Application;

    /// <summary>
    /// Contract for the OWIN application context.
    /// </summary>
    public interface IOwinAppContext : IAppContext
    {
        /// <summary>
        /// Gets the OWIN application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        IAppBuilder AppBuilder { get; }
    }
}