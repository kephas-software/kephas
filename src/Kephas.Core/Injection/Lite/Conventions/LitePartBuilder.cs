// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LitePartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection.Conventions;
using Kephas.Services;

namespace Kephas.Injection.Lite.Conventions
{
    /// <summary>
    /// A lightweight part builder.
    /// </summary>
    internal class LitePartBuilder : IPartBuilder
    {
        private readonly LiteRegistrationBuilder descriptorBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LitePartBuilder"/> class.
        /// </summary>
        /// <param name="descriptorBuilder">The descriptor builder.</param>
        internal LitePartBuilder(LiteRegistrationBuilder descriptorBuilder)
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
            this.descriptorBuilder.Lifetime = AppServiceLifetime.Singleton;
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
            this.descriptorBuilder.Lifetime = AppServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder AllowMultiple(bool value)
        {
            this.descriptorBuilder.AllowMultiple = value;
            return this;
        }
    }
}