// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// A MEF part builder.
    /// </summary>
    public class SystemCompositionPartBuilder : IPartBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionPartBuilder"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instance">The instance.</param>
        public SystemCompositionPartBuilder(Type contractType, object instance)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(instance, nameof(instance));

            this.ContractType = contractType;
            this.Instance = instance;
            this.IsSingleton = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionPartBuilder"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instanceFactory">The instance factory.</param>
        public SystemCompositionPartBuilder(Type contractType, Func<IInjector, object> instanceFactory)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(instanceFactory, nameof(instanceFactory));

            this.ContractType = contractType;
            this.InstanceFactory = instanceFactory;
        }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type ContractType { get; }

        /// <summary>
        /// Gets the instance factory.
        /// </summary>
        /// <value>
        /// The instance factory.
        /// </value>
        public Func<IInjector, object> InstanceFactory { get; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance { get; }

        /// <summary>
        /// Gets a value indicating whether this object is shared.
        /// </summary>
        /// <value>
        /// True if this object is shared, false if not.
        /// </value>
        public bool IsSingleton { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object is scoped.
        /// </summary>
        /// <value>
        /// True if this object is scoped, false if not.
        /// </value>
        public bool IsScoped { get; private set; }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartBuilder Singleton()
        {
            this.IsSingleton = true;
            this.IsScoped = false;
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
            this.IsSingleton = false;
            this.IsScoped = true;
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
            // this is not used
            return this;
        }
    }
}