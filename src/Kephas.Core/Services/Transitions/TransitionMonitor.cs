// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransitionMonitor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Class monitoring the state of a service transition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Transitions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Transitioning;

    /// <summary>
    /// Class monitoring the state of a service transition.
    /// </summary>
    public class TransitionMonitor : ITransitionState
    {
        /// <summary>
        /// The transition name.
        /// </summary>
        private readonly string transitionName;

        /// <summary>
        /// The service name.
        /// </summary>
        private readonly string serviceName;

        /// <summary>
        /// The synchronization object.
        /// </summary>
        private readonly object syncObject = new object();

        /// <summary>
        /// Stores the progess flag.
        /// null: not started.
        /// true: in progress.
        /// false: completed.
        /// </summary>
        private bool? inProgress;

        /// <summary>
        /// Stores the faulted flag.
        /// </summary>
        private bool isFaulted;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMonitor"/> class.
        /// </summary>
        /// <param name="transitionName">Name of the transition.</param>
        public TransitionMonitor(string transitionName)
        {
            this.transitionName = transitionName;
            // ReSharper disable once VirtualMemberCallInContructor
            serviceName = GetServiceName();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMonitor"/> class.
        /// </summary>
        /// <param name="transitionName">Name of the transition.</param>
        /// <param name="serviceName">Name of the service.</param>
        public TransitionMonitor(string transitionName, string serviceName)
        {
            this.transitionName = transitionName;
            // ReSharper disable once VirtualMemberCallInContructor
            this.serviceName = serviceName ?? GetServiceName();
        }

        /// <summary>
        /// Gets a value indicating whether the transition is not started.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is not started; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotStarted => inProgress == null;

        /// <summary>
        /// Gets a value indicating whether the transition is in progress.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is in progress; otherwise, <c>false</c>.
        /// </value>
        public bool IsInProgress => inProgress.HasValue && inProgress.Value;

        /// <summary>
        /// Gets a value indicating whether the transition is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted => inProgress.HasValue && !inProgress.Value;

        /// <summary>
        /// Gets a value indicating whether the transition is completed succcessfully.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition  is completed succcessfully; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompletedSuccessfully => inProgress.HasValue && !inProgress.Value && !IsFaulted;

        /// <summary>
        /// Gets a value indicating whether the transition is faulted.
        /// </summary>
        /// <value>
        /// <c>true</c> if the transition is faulted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFaulted => isFaulted;

        /// <summary>
        /// Gets the exception in the case the transitioning is faulted.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Asserts that the service is not started.
        /// </summary>
        public void AssertIsNotStarted()
        {
            if (!IsNotStarted)
            {
                throw new ServiceTransitionException(string.Format(Resources.Strings.TransitionMonitor_AssertIsNotStarted_Exception, transitionName, serviceName));
            }
        }

        /// <summary>
        /// Asserts that the service is in progress.
        /// </summary>
        public void AssertIsInProgress()
        {
            if (!IsInProgress)
            {
                throw new ServiceTransitionException(string.Format(Resources.Strings.TransitionMonitor_AssertIsInProgress_Exception, transitionName, serviceName));
            }
        }

        /// <summary>
        /// Asserts that the service is completed.
        /// </summary>
        public void AssertIsCompleted()
        {
            if (!IsCompleted)
            {
                throw new ServiceTransitionException(string.Format(Resources.Strings.TransitionMonitor_AssertIsCompleted_Exception, transitionName, serviceName));
            }
        }

        /// <summary>
        /// Asserts that the service is completed successfully.
        /// </summary>
        public void AssertIsCompletedSuccessfully()
        {
            if (!IsCompleted || IsFaulted)
            {
                throw new ServiceTransitionException(string.Format(Resources.Strings.TransitionMonitor_AssertIsCompletedSuccessfully_Exception, transitionName, serviceName));
            }
        }

        /// <summary>
        /// Starts the transition.
        /// </summary>
        public void Start()
        {
            AssertIsNotStarted();

            lock (syncObject)
            {
                AssertIsNotStarted();
                inProgress = true;
            }
        }

        /// <summary>
        /// Completes the transition successfully.
        /// </summary>
        public void Complete()
        {
            if (IsCompletedSuccessfully)
            {
                return;
            }

            AssertIsInProgress();

            lock (syncObject)
            {
                AssertIsInProgress();
                inProgress = false;
            }
        }

        /// <summary>
        /// Marks the transition as faulted.
        /// </summary>
        /// <param name="exception">The exception which occured.</param>
        public void Fault(Exception exception)
        {
            Requires.NotNull(exception, nameof(exception));

            if (IsFaulted)
            {
                return;
            }

            AssertIsInProgress();

            lock (syncObject)
            {
                AssertIsInProgress();
                inProgress = false;
                isFaulted = true;
                Exception = exception;
            }
        }

        /// <summary>
        /// Resets the transition to its initial state.
        /// </summary>
        public void Reset()
        {
            lock (syncObject)
            {
                inProgress = null;
                isFaulted = false;
            }
        }

        /// <summary>
        /// Gets service name.
        /// </summary>
        /// <returns>
        /// The service name.
        /// </returns>
        protected virtual string GetServiceName()
        {
            return $"[{GetType().Name}]";
        }
    }

    /// <summary>
    /// Class monitoring the state of a transition for the service <typeparamref name="TContract"/> with the implementation <typeparamref name="TServiceImplementation"/>.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TServiceImplementation">The service implementation type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class TransitionMonitor<TContract, TServiceImplementation> : TransitionMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMonitor{TContract, TServiceImplementation}" /> class.
        /// </summary>
        /// <param name="transitionName">Name of the transition.</param>
        public TransitionMonitor(string transitionName)
            : base(transitionName, ComputeServiceName())
        {
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <returns>The name of the service.</returns>
        private static string ComputeServiceName()
        {
            return $"{typeof(TContract).Name} [{typeof(TServiceImplementation).Name}]";
        }
    }

    /// <summary>
    /// Class monitoring the state of a transition for the service <typeparamref name="TContract"/> with the implementation type provided in constructor.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class TransitionMonitor<TContract> : TransitionMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMonitor{TContract}" /> class.
        /// </summary>
        /// <param name="transitionName">Name of the transition.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        public TransitionMonitor(string transitionName, Type serviceImplementationType)
            : base(transitionName, ComputeServiceName(serviceImplementationType))
        {
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <returns>
        /// The name of the service.
        /// </returns>
        private static string ComputeServiceName(Type serviceImplementationType)
        {
            return $"{typeof(TContract).Name} [{serviceImplementationType.Name}]";
        }
    }
}