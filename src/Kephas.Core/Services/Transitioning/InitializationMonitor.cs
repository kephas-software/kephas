// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationMonitor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Class monitoring the initialization state for the service <typeparamref name="TContract" /> with the implementation <typeparamref name="TServiceImplementation" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitioning
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Class monitoring the initialization state for the service <typeparamref name="TContract"/> with the implementation <typeparamref name="TServiceImplementation"/>.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TServiceImplementation">The service implementation type.</typeparam>
    public class InitializationMonitor<TContract, TServiceImplementation> : TransitionMonitor<TContract, TServiceImplementation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationMonitor{TContract, TServiceImplementation}"/> class.
        /// </summary>
        public InitializationMonitor()
            : base("initialization")
        {
        }
    }

    /// <summary>
    /// Class monitoring the initialization state for the service <typeparamref name="TContract"/> with the implementation type provided in constructor.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class InitializationMonitor<TContract> : TransitionMonitor<TContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationMonitor{TContract}"/> class.
        /// </summary>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        public InitializationMonitor(Type serviceImplementationType)
            : base("initialization", serviceImplementationType)
        {
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));
        }
    }
}