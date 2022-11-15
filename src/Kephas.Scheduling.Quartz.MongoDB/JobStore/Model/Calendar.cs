// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Calendar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the calendar class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.MongoDB.JobStore.Model
{
    using Kephas.Activation;
    using Kephas.Scheduling.Quartz.JobStore.Model;

    /// <summary>
    /// Implementation of a <see cref="ICalendar"/> entity.
    /// </summary>
    [ImplementationFor(typeof(ICalendar))]
    public class Calendar : QuartzEntityBase, ICalendar
    {
        /// <summary>
        /// Gets or sets the name of the calendar.
        /// </summary>
        /// <value>
        /// The name of the calendar.
        /// </value>
        public string CalendarName { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public byte[] Content { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{typeof(Calendar).Name} {this.CalendarName}/{this.InstanceName}";
        }
    }
}