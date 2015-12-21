// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidBehaviorRule.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Value rule contract for the valid behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using Kephas.Behaviors;

    /// <summary>
    /// Value rule contract for the valid behavior.
    /// </summary>
    public interface IValidBehaviorRule : IAsyncModelBehaviorRule<bool>
    {
    }
}