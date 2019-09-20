// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Conventions
{
    using Kephas.Composition.Conventions;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A Microsoft.Extensions.DependencyInjection part builder.
    /// </summary>
    public class DependencyInjectionPartBuilder : IPartBuilder
    {
        private readonly ServiceDescriptorBuilder descriptorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionPartBuilder"/> class.
        /// </summary>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        internal DependencyInjectionPartBuilder(ServiceDescriptorBuilder descriptorBuilder)
        {
            this.descriptorBuilder = descriptorBuilder;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Singleton()
        {
            this.descriptorBuilder.Lifetime = ServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Scoped()
        {
            this.descriptorBuilder.Lifetime = ServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder IPartBuilder.AllowMultiple(bool value)
        {
            // By default, all registrations are multiple.
            return this;
        }
    }
}