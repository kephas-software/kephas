// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Marks an interface to be an application service.
//   Application services are automatically identified by the composition
//   and added to the container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Marks an interface to be an application service contract. 
    /// Application services are automatically identified by the composition
    /// and added to the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AppServiceContractAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        public AppServiceContractAttribute()
            : this(AppServiceLifetime.Shared)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceContractAttribute"/> class.
        /// </summary>
        /// <param name="lifetime">The lifetime.</param>
        public AppServiceContractAttribute(AppServiceLifetime lifetime)
        {
            this.Lifetime = lifetime;
        }

        /// <summary>
        /// Gets the application service lifetime.
        /// </summary>
        /// <value>
        /// The application service lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple services for this contract are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple services are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowMultiple { get; set; }

        /// <summary>
        /// Gets a value indicating whether the application service <see cref="AppServiceContractAttribute"/> is shared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shared; otherwise, <c>false</c>.
        /// </value>
        public bool IsShared
        {
            get { return this.Lifetime == AppServiceLifetime.Shared; }
        }

        /// <summary>
        /// Gets a value indicating whether the application service <see cref="AppServiceContractAttribute"/> is instanced per request.
        /// </summary>
        /// <value>
        ///   <c>true</c> if instanced per request; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstance
        {
            get { return this.Lifetime == AppServiceLifetime.Instance; }
        }
    }
}