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
    using System.Collections.ObjectModel;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// Information about the application.
    /// </summary>
    public class AppInfo : DynamicTypeInfo, IAppInfo
    {
        private static readonly IEnumerable<object> EmptyAnnotations = new ReadOnlyCollection<object>(new List<object>());
        private static readonly IEnumerable<IParameterInfo> EmptyParameters = new ReadOnlyCollection<IParameterInfo>(new List<IParameterInfo>());

        private readonly AppIdentity identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfo"/> class.
        /// </summary>
        /// <param name="appIdentity">The application identity.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        public AppInfo(AppIdentity appIdentity, string description = null, string[] tags = null)
        {
            Requires.NotNull(appIdentity, nameof(AppIdentity));

            this.identity = appIdentity;
            this.FullName = this.Name = appIdentity.Id;
            this.Version = appIdentity.Version;
            this.Description = description;
#if NET45
            this.Tags = tags ?? new string[0];
#else
            this.Tags = tags ?? Array.Empty<string>();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">Optional. The version.</param>
        /// <param name="description">Optional. The description.</param>
        /// <param name="tags">Optional. The tags.</param>
        public AppInfo(string name, string version = null, string description = null, string[] tags = null)
            : this(new AppIdentity(name, version), description, tags)
        {
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string Version { get; }

        /// <summary>
        /// Gets the application description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

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
        IEnumerable<IParameterInfo> IAppInfo.Parameters => EmptyParameters;

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public IEnumerable<IAppDependency> Dependencies { get; } = new List<IAppDependency>();

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <returns>
        /// The identity.
        /// </returns>
        public AppIdentity GetIdentity() => this.identity;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.GetIdentity().ToString();
        }
    }
}
