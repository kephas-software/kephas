// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationMonitor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitions
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class monitoring the initialization state for the service <typeparamref name="TContract"/> with the implementation <typeparamref name="TService"/>.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    /// <typeparam name="TService">The service implementation type.</typeparam>
    public class InitializationMonitor<TContract, TService> : TransitionMonitor<TContract, TService>
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
    public class InitializationMonitor<TContract> : TransitionMonitor<TContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationMonitor{TContract}"/> class.
        /// </summary>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        public InitializationMonitor(Type serviceImplementationType)
            : base("initialization", serviceImplementationType)
        {
            serviceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));
        }
    }
}