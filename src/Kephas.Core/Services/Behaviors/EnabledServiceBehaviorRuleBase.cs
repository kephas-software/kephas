﻿// --------------------------------------------------------------------------------------------------------------------
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
    /// It applies for services implementing the contract <typeparamref name="TContract"/>.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    public abstract class EnabledServiceBehaviorRuleBase<TContract> : BehaviorRuleBase<IServiceBehaviorContext<TContract>, bool>, IEnabledServiceBehaviorRule<TContract>
        where TContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledServiceBehaviorRuleBase{TContract}"/> class.
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
            return this.CanApply((IServiceBehaviorContext<TContract>)context);
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
            return this.GetValue((IServiceBehaviorContext<TContract>)context);
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
            return this.GetValue((IServiceBehaviorContext<TContract>)context);
        }
    }

    /// <summary>
    /// Base class for behavior rules controlling the enabled state of the service <typeparamref name="TService"/>.
    /// The service must implement the contract <typeparamref name="TContract"/>.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TService">Type of the service implementation.</typeparam>
    public abstract class EnabledServiceBehaviorRuleBase<TContract, TService> : EnabledServiceBehaviorRuleBase<TContract>
        where TService : TContract
        where TContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledServiceBehaviorRuleBase{TContract, TService}"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected EnabledServiceBehaviorRuleBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Base class for behavior rules controlling the enabled state of services
        /// implementing the contract <typeparamref name="TContract"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A value indicating whether the rule may be applied or not.
        /// </returns>
        public override bool CanApply(IServiceBehaviorContext<TContract> context)
        {
            // TODO clarify whether this check will apply to the class hierarchy or to the type itself
            return context.Service?.GetType() == typeof(TService);
        }
    }
}