// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedAppServiceContractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marks an interface to be a shared application service.
//   Application services are automatically identified by the composition
//   and added to the container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Marks an interface to be a shared application service contract.
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SharedAppServiceContractAttribute : AppServiceContractAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedAppServiceContractAttribute"/> class.
        /// </summary>
        public SharedAppServiceContractAttribute()
            : base(AppServiceLifetime.Shared)
        {
        }
    }
}