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

    /// <summary>
    /// Marks an interface to be a shared application service contract within a specific scope.
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ScopeSharedAppServiceContractAttribute : AppServiceContractAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeSharedAppServiceContractAttribute"/> class.
        /// </summary>
        public ScopeSharedAppServiceContractAttribute()
            : base(AppServiceLifetime.ScopeShared)
        {
        }
    }
}