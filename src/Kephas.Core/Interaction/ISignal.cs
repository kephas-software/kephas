// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISignal interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Interaction
{
    using Kephas.ExceptionHandling;

    /// <summary>
    /// Marker interface for signals.
    /// </summary>
    /// <remarks>
    /// Signals may be implemented as a special kind of exceptions
    /// because, in some cases, they must interrupt the normal flow.
    /// </remarks>
    public interface ISignal : ISeverityQualifiedException
    {
    }
}