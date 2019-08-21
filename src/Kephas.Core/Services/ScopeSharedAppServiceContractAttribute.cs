// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopeSharedAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scope shared application service contract attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Marks an interface to be contract for singleton application services within a specific scope.
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [Obsolete("Please use ScopedAppServiceContractAttribute instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ScopeSharedAppServiceContractAttribute : AppServiceContractAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeSharedAppServiceContractAttribute"/> class.
        /// </summary>
        public ScopeSharedAppServiceContractAttribute()
            : base(AppServiceLifetime.Scoped)
        {
        }
    }
}