// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceBehaviorRuleBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for behavior rules controlling the enabled state of services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using Kephas.Behaviors;
    using Kephas.Logging;

    /// <summary>
    /// Base class for behavior rules controlling the enabled state of services.
    /// It applyes for services implementing the contract <typeparamref name="TServiceContract"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    public abstract class EnabledServiceBehaviorRuleBase<TServiceContract> : BehaviorRuleBase<IServiceBehaviorContext<TServiceContract>, bool>, IEnabledServiceBehaviorRule<TServiceContract>
        where TServiceContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledServiceBehaviorRuleBase{TServiceContract}"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected EnabledServiceBehaviorRuleBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the rule may be applied or not.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A value indicating whether the rule may be applied or not.
        /// </returns>
        bool IBehaviorRule<IContext>.CanApply(IContext context)
        {
            return this.CanApply((IServiceBehaviorContext<TServiceContract>)context);
        }

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        IBehaviorValue<bool> IBehaviorRule<IContext, bool>.GetValue(IContext context)
        {
            return this.GetValue((IServiceBehaviorContext<TServiceContract>)context);
        }

        /// <summary>
        /// Gets the behavior value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The behavior value.
        /// </returns>
        IBehaviorValue IBehaviorRule<IContext>.GetValue(IContext context)
        {
            return this.GetValue((IServiceBehaviorContext<TServiceContract>)context);
        }
    }

    /// <summary>
    /// Base class for behavior rules controlling the enabled state of the service <typeparamref name="TServiceImplementation"/>.
    /// The service must implement the contract <typeparamref name="TServiceContract"/>.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    /// <typeparam name="TServiceImplementation">Type of the service implementation.</typeparam>
    public abstract class EnabledServiceBehaviorRuleBase<TServiceContract, TServiceImplementation> : EnabledServiceBehaviorRuleBase<TServiceContract>
        where TServiceImplementation : TServiceContract
        where TServiceContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledServiceBehaviorRuleBase{TServiceContract, TServiceImplementation}"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected EnabledServiceBehaviorRuleBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Base class for behavior rules controlling the enabled state of services
        /// implementing the contract <typeparamref name="TServiceContract"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A value indicating whether the rule may be applied or not.
        /// </returns>
        public override bool CanApply(IServiceBehaviorContext<TServiceContract> context)
        {
            // TODO clarify whether this check will apply to the class hierarchy or to the type itself
            return context.Service?.GetType() == typeof(TServiceImplementation);
        }
    }
}