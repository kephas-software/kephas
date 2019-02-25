// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITrigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITrigger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for trigger.
    /// </summary>
    public interface ITrigger : IExpando, IInstance
    {
    }
}