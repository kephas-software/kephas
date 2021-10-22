// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// Information about the application.
    /// </summary>
    public class AppInfo : DynamicTypeInfo, IAppInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfo"/> class.
        /// </summary>
        /// <param name="identity">The app identity.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        public AppInfo(AppIdentity identity, string? description = null, string[]? tags = null)
        {
            identity = identity ?? throw new ArgumentNullException(nameof(identity));

            this.Id = this.Identity = identity;
            this.FullName = this.Name = identity.Id;
            this.Description = description;
            this.Tags = tags ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">Optional. The version.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        public AppInfo(string name, string? version = null, string? description = null, string[]? tags = null)
            : this(new AppIdentity(name, version), description, tags)
        {
        }

        /// <summary>
        /// Gets the app identity.
        /// </summary>
        /// <value>
        /// The app identity.
        /// </value>
        public AppIdentity Identity { get; }

        /// <summary>
        /// Gets the application description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string? Description { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string[] Tags { get; }

        /// <summary>
        /// Gets the application parameters.
        /// </summary>
        /// <value>
        /// The application parameters.
        /// </value>
        IEnumerable<IParameterInfo> IAppInfo.Parameters => Enumerable.Empty<IParameterInfo>();

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public IEnumerable<IAppDependency> Dependencies { get; } = new List<IAppDependency>();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Identity.ToString();
        }
    }
}
