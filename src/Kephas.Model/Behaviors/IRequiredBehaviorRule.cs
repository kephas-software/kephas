// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequiredBehaviorRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Value rule contract for the required behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using Kephas.Behaviors;

    /// <summary>
    /// Value rule contract for the required behavior.
    /// </summary>
    public interface IRequiredBehaviorRule : IAsyncModelBehaviorRule<bool>
    {
    }
}