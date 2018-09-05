// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetFeature.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the OWIN feature class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Application
{
    /// <summary>
    /// The OWIN feature.
    /// </summary>
    public static class AspNetFeature
    {
        /// <summary>
        /// Gets the ASP.NET feature.
        /// </summary>
        /// <value>
        /// The ASP.NET feature.
        /// </value>
        public const string WebHost = nameof(WebHost);
    }
}