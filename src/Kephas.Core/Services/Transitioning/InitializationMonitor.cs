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
}