// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Application.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for application information.
    /// </summary>
    public interface IAppInfo : ITypeInfo
    {
        /// <summary>
        /// Gets the app identity.
        /// </summary>
        /// <value>
        /// The app identity.
        /// </value>
        AppIdentity Identity { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string[] Tags { get; }

        /// <summary>
        /// Gets the application description.
        /// </summary>
        /// <value>
        /// The application description.
        /// </value>
        string? Description { get; }

        /// <summary>
        /// Gets the application parameters.
        /// </summary>
        /// <value>
        /// The application parameters.
        /// </value>
        IEnumerable<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        IEnumerable<IAppDependency> Dependencies { get; }
    }
}
