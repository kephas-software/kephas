// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnabledServiceBehaviorRule.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Behavior rule contract for controlling the enabled state of services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using Kephas.Behavior;

    /// <summary>
    /// Interface for enabled service behavior rule.
    /// </summary>
    public interface IEnabledServiceBehaviorRule : IBehaviorRule<IContext, bool>
    {
    }

    /// <summary>
    /// Behavior rule contract for controlling the enabled state of services.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    [SharedAppServiceContract(ContractType = typeof(IEnabledServiceBehaviorRule), AllowMultiple = true)]
    public interface IEnabledServiceBehaviorRule<in TServiceContract> : IBehaviorRule<IServiceBehaviorContext<TServiceContract>, bool>, IEnabledServiceBehaviorRule
    {
    }
}