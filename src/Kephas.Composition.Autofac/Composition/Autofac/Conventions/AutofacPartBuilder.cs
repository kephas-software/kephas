// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Conventions
{
    using global::Autofac.Builder;

    using Kephas.Composition.Conventions;

    /// <summary>
    /// An Autofac part builder.
    /// </summary>
    public class AutofacPartBuilder : IPartBuilder
    {
        private readonly IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacPartBuilder"/> class.
        /// </summary>
        /// <param name="registrationBuilder">The registration builder.</param>
        public AutofacPartBuilder(IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder)
        {
            this.registrationBuilder = registrationBuilder;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Shared()
        {
            this.registrationBuilder.SingleInstance();
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <param name="scopeName">Optional. Name of the scope.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder ScopeShared(string scopeName = CompositionScopeNames.Default)
        {
            this.registrationBuilder.InstancePerLifetimeScope();
            return this;
        }
    }
}