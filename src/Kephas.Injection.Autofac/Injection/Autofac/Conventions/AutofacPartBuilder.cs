﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Conventions
{
    using System;
    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// An Autofac part builder.
    /// </summary>
    public class AutofacPartBuilder : IPartBuilder
    {
        private readonly ContainerBuilder containerBuilder;

        private readonly IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder;
        private bool allowMultiple;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacPartBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="registrationBuilder">The registration builder.</param>
        public AutofacPartBuilder(ContainerBuilder containerBuilder, IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder)
        {
            this.containerBuilder = containerBuilder;
            this.registrationBuilder = registrationBuilder;
        }

        /// <summary>
        /// Marks the part as singleton within the injection.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Singleton()
        {
            this.registrationBuilder.SingleInstance();
            return this;
        }

        /// <summary>
        /// Mark the part as singleton within a scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder Scoped()
        {
            this.registrationBuilder.InstancePerLifetimeScope();
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
            this.allowMultiple = value;
            return this;
        }

        /// <summary>
        /// Builds the information into a service descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        public void Build()
        {
            var registration = this.registrationBuilder.CreateRegistration();
            this.containerBuilder.RegisterComponent(registration);
        }
    }
}