// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAspNetAppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAspNetAppContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Application
{
    using Kephas.Application;

    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Contract for the OWIN application context.
    /// </summary>
    public interface IAspNetAppContext : IAppContext
    {
        /// <summary>
        /// Gets the OWIN application builder.
        /// </summary>
        /// <value>
        /// The application builder.
        /// </value>
        IApplicationBuilder AppBuilder { get; }
    }
}