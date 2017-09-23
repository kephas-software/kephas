// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeSharedAppServiceContractAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the scoped application service contract attribute class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Marks an interface to be a shared application service contract within a specific scope.
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ScopeSharedAppServiceContractAttribute : AppServiceContractAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeSharedAppServiceContractAttribute"/> class.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        public ScopeSharedAppServiceContractAttribute(string scopeName = ScopeNames.Default)
            : base(AppServiceLifetime.ScopeShared)
        {
            Requires.NotNullOrEmpty(scopeName, nameof(scopeName));

            this.ScopeName = scopeName;
        }

        /// <summary>
        /// Gets the name of the scope.
        /// </summary>
        /// <value>
        /// The name of the scope.
        /// </value>
        public string ScopeName { get; }
    }
}