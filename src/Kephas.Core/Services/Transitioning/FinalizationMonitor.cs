// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalizationMonitor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the finalization monitor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitioning
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Class monitoring the finalization state for the service <typeparamref name="TContract"/> with the implementation <typeparamref name="TServiceImplementation"/>.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TServiceImplementation">The service implementation type.</typeparam>
    public class FinalizationMonitor<TContract, TServiceImplementation> : TransitionMonitor<TContract, TServiceImplementation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalizationMonitor{TContract, TServiceImplementation}"/> class.
        /// </summary>
        public FinalizationMonitor()
            : base("finalization")
        {
        }
    }

    /// <summary>
    /// Class monitoring the finalization state for the service <typeparamref name="TContract"/> with the implementation type provided in constructor.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class FinalizationMonitor<TContract> : TransitionMonitor<TContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalizationMonitor{TContract}"/> class.
        /// </summary>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        public FinalizationMonitor(Type serviceImplementationType)
            : base("finalization", serviceImplementationType)
        {
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));
        }
    }
}