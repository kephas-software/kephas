// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationMonitor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Class monitoring the initialization state for the service <typeparamref name="TContract" /> with the implementation <typeparamref name="TServiceImplementation" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitioning
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

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
            Contract.Requires(serviceImplementationType != null);
        }
    }
}