// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentifiableEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the identifiable event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using Kephas.Dynamic;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Default implementation of an identifiable event.
    /// </summary>
    [ExcludeFromModel]
    public class IdentifiableEvent : IIdentifiableEvent
    {
        /// <summary>
        /// Gets or sets the identifier for this event.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public object Id { get; set; }

        /// <summary>
        /// Gets or sets the event arguments.
        /// </summary>
        /// <value>
        /// The event arguments.
        /// </value>
        public IExpando EventArgs { get; set; }
    }
}