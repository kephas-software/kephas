// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnabledServiceBehaviorRule.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Behavior rule contract for controlling the enabled state of services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Behaviors;

    /// <summary>
    /// Interface for enabled service behavior rule.
    /// </summary>
    public interface IEnabledServiceBehaviorRule : IBehaviorRule<IContext, bool>
    {
    }

    /// <summary>
    /// Behavior rule contract for controlling the enabled state of services.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    [SingletonAppServiceContract(ContractType = typeof(IEnabledServiceBehaviorRule), AllowMultiple = true)]
    public interface IEnabledServiceBehaviorRule<in TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] in TMetadata> : IBehaviorRule<IServiceBehaviorContext<TContract, TMetadata>, bool>, IEnabledServiceBehaviorRule
        where TContract : class
    {
    }
}