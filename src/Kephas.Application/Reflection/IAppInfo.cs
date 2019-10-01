// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for application information.
    /// </summary>
    public interface IAppInfo : IElementInfo
    {
        /// <summary>
        /// Gets the application parameters.
        /// </summary>
        /// <value>
        /// The application parameters.
        /// </value>
        IEnumerable<IParameterInfo> Parameters { get; }
    }
}
