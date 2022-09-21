// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Interaction
{
    using System;

    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// Context for the interaction.
    /// </summary>
    public class InteractionContext : Context
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public InteractionContext(IInjector injector)
            : base(injector)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating a delay in publishing the event to the subscribers.
        /// </summary>
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the publishing is one way.
        /// </summary>
        public bool IsOneWay { get; set; }

        /// <summary>
        /// Sets the publishing to one way.
        /// </summary>
        /// <returns>This context.</returns>
        public InteractionContext OneWay()
        {
            this.IsOneWay = true;
            return this;
        }

        /// <summary>
        /// Sets the indicated delay.
        /// </summary>
        /// <param name="value">The delay.</param>
        /// <returns>This context.</returns>
        public InteractionContext WithDelay(TimeSpan value)
        {
            this.Delay = value;
            return this;
        }
    }
}